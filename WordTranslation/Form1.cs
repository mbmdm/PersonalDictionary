using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonalDictionary
{
    public partial class Form1 : Form, IApplet
    {
        DB db = DB.GetInstance();

        const int amountWords = 10;
        const int amountButtons = 5;

        static Random rundomWord, rundomButton, rundomTranslationWord;

        List<Word> wordsList;
        List<Button> buttonsList;

        int indexWord;
        int indexRundomButton;

        int counterWords, rightChoices, wrongChoices;

        //AppletData myApplet;

        public Form1()
        {
            InitializeComponent();

            wordsList = db.Words;
            buttonsList = new List<Button>(amountButtons);
            
            rundomWord = new Random();
            rundomButton = new Random();
            rundomTranslationWord = new Random();

            buttonsList.Add(button0);
            buttonsList.Add(button1);
            buttonsList.Add(button2);
            buttonsList.Add(button3);
            buttonsList.Add(button4);

            //GetAppletID();
            this.FormClosing += Form1_FormClosing;
            
            NewTraining();
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            e.Cancel = true;
            this.Hide();
        }

        //private void GetAppletID()
        //{            
        //    var applets = db.ApplestsData;
        //    myApplet = null;

        //    foreach (var item in applets)
        //    {
        //        if (item.AppletID == "dictionary_applet.PersonalDictionary.WordTranslation")
        //        {
        //            myApplet = item;
        //            break;
        //        }
        //    }

        //    if (myApplet == null) throw new Exception("AppletID not received");
        //}

        private void NewTraining()
        {
            counterWords = rightChoices = wrongChoices = 0;
            NewWord();
        }

        private void NewWord()
        {
            counterWords++;

            label5.Text = String.Format("Слово: {0}/{1}", counterWords, amountWords);

            button6.Text = "Не знаю :(";

            button6.BackColor = System.Drawing.SystemColors.Control;

            label3.Text = "";

            OnButtons();

            indexWord = rundomWord.Next(0, wordsList.Count - 1);
            indexRundomButton = rundomButton.Next(0, amountButtons);

            // На время тестирования
            label4.Text = "Информация на время тестирования:\n" + wordsList[indexWord].ToString();


            label2.Text = wordsList[indexWord].En;


            for (int number = 0; number < amountButtons; number++)
            {
                if (number == indexRundomButton)
                {
                    buttonsList[indexRundomButton].Text = wordsList[indexWord].Ru;
                    continue;
                }

                int indexTranslationWord;

                do
                {
                    indexTranslationWord = rundomTranslationWord.Next(0, wordsList.Count - 1);
                }
                while (indexTranslationWord == indexWord);

                buttonsList[number].Text = wordsList[indexTranslationWord].Ru;

            }
        }

        private void OnButtons()
        {
            foreach (Button a in buttonsList)
            {
                a.Enabled = true;
                a.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void CheckSelection(int numberButton)
        {
            if (numberButton == indexRundomButton)
                RightChoice(numberButton);
            else if (numberButton < 0)
                NoChoice();
            else
                WrongChoice(numberButton);
        }

        private void NoChoice()
        {
            wrongChoices++;
            //PushProgress(0);
            OffButtons();
        }

        //private void PushProgress(int progress)
        //{
        //    AppletDataInfo info = new AppletDataInfo();
        //    info.AppletData = myApplet;
        //    info.Word = wordsList[indexWord];

        //    if (progress == 0)
        //        info.Progress = progress;
        //    else
        //        info.Progress =  +progress;

        //    db.Push(info);
        //}

        private void WrongChoice(int numberButton)
        {
            wrongChoices++;
            //PushProgress(0);
            label3.Text = "Неверно.";
            label3.ForeColor = System.Drawing.Color.Red;
            buttonsList[numberButton].BackColor = System.Drawing.Color.IndianRed;
            OffButtons();

        }

        private void RightChoice(int numberButton)
        {
            rightChoices++;
            //PushProgress(25);
            label3.Text = "Верно.";
            label3.ForeColor = System.Drawing.Color.DarkGreen;
        OffButtons();
        }

        private void OffButtons()
        {
            button6.Text = "Следующее слово -->";
            button6.BackColor = System.Drawing.SystemColors.ActiveCaption;

            buttonsList[indexRundomButton].BackColor = System.Drawing.Color.PaleGreen;

            foreach(Button a in buttonsList)
            {
                a.Enabled = false;
            }
        }

        public string DisplayName()
        {
            return "Слово-перевод";
        }

        public bool IsMainDialog()
        {
            return false;
        }

        private void button0_Click(object sender, EventArgs e)
        {
            CheckSelection(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckSelection(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckSelection(2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CheckSelection(3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CheckSelection(4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (counterWords == amountWords)
            {
                db.Commit();

                string textResult = String.Format("Тренировка окончена.\n\nИз  {0}  слов\nверно выбрано:\t{1}\nневерно выбрано:\t{2}\n\nПовторить тренировку?",
                                                    amountWords, rightChoices, wrongChoices);

                DialogResult dr = MessageBox.Show(textResult, "Слово-перевод", MessageBoxButtons.OKCancel,
                                  MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);

                if (dr == DialogResult.OK)
                {
                    NewTraining();
                    return;
                }                 

                if (dr == DialogResult.Cancel)
                    Application.Exit();
            }

            if (button6.Text == "Не знаю :(")
                    CheckSelection(-1);
            else
               NewWord();
            
        }

        public int Position()
        {
            return 10;
        }

        public void Run()
        {
            this.ShowDialog();
        }
    }
}
