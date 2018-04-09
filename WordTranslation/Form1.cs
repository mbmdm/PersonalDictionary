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

        List<Word> tempList;
        List<Button> buttonsList;

        int indexWord;
        int indexRundomButton;

        public Form1()
        {
            InitializeComponent();

            tempList = db.Words;
            buttonsList = new List<Button>(amountButtons);
            
            rundomWord = new Random();
            rundomButton = new Random();
            rundomTranslationWord = new Random();

            buttonsList.Add(button0);
            buttonsList.Add(button1);
            buttonsList.Add(button2);
            buttonsList.Add(button3);
            buttonsList.Add(button4);

            NewWord();
            
        }

        private void NewWord()
        {
            label3.Text = "";

            OnButtons();

            indexWord = rundomWord.Next(0, tempList.Count - 1);
            indexRundomButton = rundomButton.Next(0, amountButtons);

            // На время тестирования
            label4.Text = "Информация на время тестирования:\n" + tempList[indexWord].ToString();


            label2.Text = tempList[indexWord].En;


            for (int number = 0; number < amountButtons; number++)
            {
                if (number == indexRundomButton)
                {
                    buttonsList[indexRundomButton].Text = tempList[indexWord].Ru;
                    continue;
                }

                int indexTranslationWord;

                do
                {
                    indexTranslationWord = rundomTranslationWord.Next(0, tempList.Count - 1);
                }
                while (indexTranslationWord == indexWord);

                buttonsList[number].Text = tempList[indexTranslationWord].Ru;

            }
        }

        private void OnButtons()
        {
            buttonsList[indexRundomButton].BackColor = System.Drawing.SystemColors.Control;

            foreach (Button a in buttonsList)
            {
                a.Enabled = true;
            }
        }

        private void CheckSelection(int numberButton)
        {
            if (numberButton == indexRundomButton)
                RightChoice(numberButton);
            else
                WrongChoice(numberButton);
        }

        private void WrongChoice(int numberButton)
        {
            label3.Text = "Не верно.";
            OffButtons();

        }

        private void RightChoice(int numberButton)
        {
            label3.Text = "Верно.";         
            OffButtons();
        }

        private void OffButtons()
        {
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
