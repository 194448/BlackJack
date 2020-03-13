using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArrayYee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //global variables
        int[] cards;
        Random r = new Random();
        int LocationInDeck;
        int[] playerHand;
        int[] dealerHand;

        TranslateTransform[] playerTranslateTransform;
        TranslateTransform[] dealerTranslateTransform;
        Rectangle[] rectanglePlayer;
        Rectangle[] rectangleDealer;
        ImageBrush[] playerSprite;
        ImageBrush[] dealerSprite;
        public MainWindow()
        {
            InitializeComponent();

            //instantiate
            cards = new int[52];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = i;
            }

            //set default values
            playerSprite = new ImageBrush[4];
            dealerSprite = new ImageBrush[4];
            LocationInDeck = 0;
            playerHand = new int[] { -1, -1, -1, -1 };
            dealerHand = new int[] { -1, -1, -1, -1 };
            playerTranslateTransform = new TranslateTransform[4];
            dealerTranslateTransform = new TranslateTransform[4];
            rectanglePlayer = new Rectangle[4];
            rectangleDealer = new Rectangle[4];


            string output = "";
            for (int i = 0; i < cards.Length; i++)
            {
                output += cards[i].ToString() + Environment.NewLine;
            }
            MessageBox.Show(output);
            cards = Shuffle(cards);
            output = "";
            for (int i = 0; i < cards.Length; i++)
            {
                output += cards[i].ToString() + Environment.NewLine;
            }
            MessageBox.Show(output);

            playGame();
        }//end MainWindow

        public int[] Shuffle(int[] c)
        {
            for (int i = 0; i < c.Length; i++)
            {
                int temp = r.Next(c.Length);
                int tempValue = c[i];
                c[i] = c[temp];
                c[temp] = tempValue;
            }
            return c;
        }

        public int Deal()
        {
            int c = cards[LocationInDeck];
            LocationInDeck++;
            return c;
        }//end Deal method

        //PlayGame
        //Deal two cards to the player
        //deal two cards to the dealer
        //show both hands.
        public void playGame()
        {
            for (int i = 0; i < 2; i++)
            {
                playerHand[i] = Deal();
                dealerHand[i] = Deal();
            }

            string playerhandOutput = "Player cards: ";
            string dealerhandOutput = "Dealer cards: ";
            for (int i = 0; i < playerHand.Length; i++)
            {
                if (playerHand[i] >= 0)
                {
                    playerhandOutput += playerHand[i].ToString() + ", ";
                }
                if (dealerHand[i] >= 0)
                {
                    dealerhandOutput += dealerHand[i].ToString() + ", ";
                }
            }
            displayCard();
            MessageBox.Show(playerhandOutput + Environment.NewLine
                + dealerhandOutput);
        }
        public void displayCard()
        {
            canvas.Children.Clear();//clear old cards
            BitmapImage bitmapImage = new BitmapImage(new Uri("cards.png", UriKind.Relative));

            for (int i = 0; i < playerHand.Length; i++)
            {
                playerSprite[i] = new ImageBrush(bitmapImage);
                dealerSprite[i] = new ImageBrush(bitmapImage);
                playerTranslateTransform[i] = new TranslateTransform(0, 0);
                dealerTranslateTransform[i] = new TranslateTransform(0, 0);

                playerSprite[i].Stretch = Stretch.None;
                playerSprite[i].AlignmentX = AlignmentX.Left;
                playerSprite[i].AlignmentY = AlignmentY.Top;

                playerSprite[i].Viewport = new Rect(0, 0, bitmapImage.Width / 13, bitmapImage.Height / 4);

                dealerSprite[i].Stretch = Stretch.None;
                dealerSprite[i].AlignmentX = AlignmentX.Left;
                dealerSprite[i].AlignmentY = AlignmentY.Top;

                dealerSprite[i].Viewport = new Rect(0, 0, bitmapImage.Width / 13, bitmapImage.Height / 4);

                //set based on card
                playerTranslateTransform[i] =
                    new TranslateTransform(-(playerHand[i] % 13) * (bitmapImage.Width / 13),
                    -(playerHand[i] / 13) * (bitmapImage.Height / 4));
                playerSprite[i].Transform = playerTranslateTransform[i];

                dealerTranslateTransform[i] =
                    new TranslateTransform(-(dealerHand[i] % 13) * (bitmapImage.Width / 13),
                    -(dealerHand[i] / 13) * (bitmapImage.Height / 4));
                dealerSprite[i].Transform = dealerTranslateTransform[i];

                //make array for player and dealer
                rectanglePlayer[i] = new Rectangle();
                rectanglePlayer[i].Fill = playerSprite[i];
                rectanglePlayer[i].Width = bitmapImage.Width / 13;
                rectanglePlayer[i].Height = bitmapImage.Height / 4;

                rectangleDealer[i] = new Rectangle();
                rectangleDealer[i].Fill = dealerSprite[i];
                rectangleDealer[i].Width = bitmapImage.Width / 13;
                rectangleDealer[i].Height = bitmapImage.Height / 4;


                canvas.Children.Add(rectangleDealer[i]);
                //set based on position of hand
                Canvas.SetTop(rectangleDealer[i], 230);
                Canvas.SetLeft(rectangleDealer[i], 10 * i + (bitmapImage.Width / 13) * i);

                canvas.Children.Add(rectanglePlayer[i]);
                //set based on position of hand
                Canvas.SetTop(rectanglePlayer[i], 30);
                Canvas.SetLeft(rectanglePlayer[i], 10 * i + (bitmapImage.Width / 13) * i);
            }//end for loop
        }

        private void btnHit_Click(object sender, RoutedEventArgs e)
        {
            for(int i =0; i < playerHand.Length; i++)
            {
                if(playerHand[i] < 0)
                {
                    playerHand[i] = Deal();
                    break;
                }//end of if loop
            }//end of for loop
            displayCard();
            CheckIfBust();
        }

        private void btnStay_Click(object sender, RoutedEventArgs e)
        {
            CheckIfBust();
            updateDealer();

        }

        private void btnDealNewHand_Click(object sender, RoutedEventArgs e)
        {

        }

        public void CheckIfBust()
        {
            //throw new notimplimented exception
        }

        public void updateDealer()
        {
            int HandTotal = 0;
            for(int i = 0; i < dealerHand.Length; i++)
            {
                int CurrentCard = dealerHand[i] % 13 + 1;
                if (CurrentCard > 10)
                {
                    CurrentCard = 10;
                }
                if (CurrentCard == 1)
                {
                    CurrentCard = 11;
                }
                HandTotal += CurrentCard;
            }//endloop
            if (HandTotal == 21)
            {
                Gameover();
            }
            else if (HandTotal > 16)
            {
                checkWin();
            }
            else
            {
                HitDealer();
            }
        }
        public void Gameover()
        {

        }

        public void checkWin()
        {

        }

        public void HitDealer()
        {

        }
    }
}//each card is 71 x 96