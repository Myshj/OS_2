using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;

namespace OS_2
{
    public partial class Form1 : Form
    {
        static Object l = new Object();

        static int common = 0;

        const string emptyText = "<пусто>";

        int[] coinNominals = new int[7];

        bool coinEntered = false;

        /*Спільний ресурс.*/
        /*Скільки монет різних номіналів є в наявності?*/
        int[] coinsAvailable = new int[7];
        /*Чи поступила нова монета?*/
        bool coinInputed = false;
        /*Якого номіналу нова монета?*/
        int typeOfNewCoin = -1;
        /*Якого номіналу монети хочемо отримати?*/
        int typeOfCoinToReceive;
        /*Скільки монет різних номіналів отримали у відповідь?*/
        int[] coinsReceived = new int[7];
        /*Кінець спільного ресурсу.*/

        Thread A;
        Thread B;

        public Form1()
        {
            InitializeComponent();

            coinsAvailable[0] = (int)kop1Available.Value;
            coinsAvailable[1] = (int)kop2Available.Value;
            coinsAvailable[2] = (int)kop5Available.Value;
            coinsAvailable[3] = (int)kop10Available.Value;
            coinsAvailable[4] = (int)kop25Available.Value;
            coinsAvailable[5] = (int)kop50Available.Value;
            coinsAvailable[6] = (int)hrn1Available.Value;

            coinNominals[0] = 1;
            coinNominals[1] = 2;
            coinNominals[2] = 5;
            coinNominals[3] = 10;
            coinNominals[4] = 25;
            coinNominals[5] = 50;
            coinNominals[6] = 100;

            A = new Thread(ThreadA);
            B = new Thread(ThreadB);
            A.Start();
            B.Start();
            //A.Join();
            //B.Join();
        }

        void ThreadA()
        {
            while (true)
            {
                int local = 1;

                while(1 == local){
                    TS(ref local, ref common);

                    Thread.Sleep(50);
                }

                
                /*Критична ділянка.*/
                if (coinEntered)
                {
                    coinInputed = true;
                    /*Яку ж монету ми отримали?*/
                    WhatCoinInputed();

                    if (typeOfNewCoin == -1) { return; }

                    /*Що ми хочемо отримати у відповідь?*/
                    WhatDoWeWantToReceive();

                    coinEntered = false;
                }

                CommonToNull();
                /*Кінець критичної ділянки.*/

                Thread.Sleep(100);
            }
        }

        void ThreadB()
        {
            while (true)
            {
                string outText = "";
                int local = 1;

                while (1 == local)
                {
                    TS(ref local, ref common);
                    Thread.Sleep(50);
                }

                /*Критична ділянка.*/
                if (coinInputed)
                {
                    bool wasError = true;
                    /*Чи вірно задан номінал?*/
                    if (typeOfNewCoin < 0) { outText = "Монети такого номіналу не приймаються!"; }
                    /*Чи можливий обмін монет таких номіналів?*/
                    else if (typeOfCoinToReceive > typeOfNewCoin) { outText = "Не можна розмінювати монети меншого номіналу на монети більшого номіналу!"; }
                    /*Чи є в наявності необхідна кількість монет?*/
                    else if (coinsAvailable[typeOfCoinToReceive] * coinNominals[typeOfCoinToReceive] < coinNominals[typeOfNewCoin]) { outText = "Немає потрібної кількості монет!"; }
                    else { outText = "Обмін проведено успішно!"; wasError = false; }

                    if (!wasError)
                    {
                        coinsAvailable[typeOfNewCoin]++;

                        int countOfCoinsToReceive = coinNominals[typeOfNewCoin] / coinNominals[typeOfCoinToReceive];
                        coinsAvailable[typeOfCoinToReceive] -= countOfCoinsToReceive;

                        if (0 == typeOfCoinToReceive) { Invoke((MethodInvoker)(() => kop1Received.Text = countOfCoinsToReceive.ToString())); 
                                                        Invoke((MethodInvoker)(() => kop1Available.Value = coinsAvailable[typeOfCoinToReceive])); }

                        else if (1 == typeOfCoinToReceive) { Invoke((MethodInvoker)(() => kop2Received.Text = countOfCoinsToReceive.ToString()));
                                                             Invoke((MethodInvoker)(() => kop2Available.Value = coinsAvailable[typeOfCoinToReceive])); }

                        else if (2 == typeOfCoinToReceive) { Invoke((MethodInvoker)(() => kop5Received.Text = countOfCoinsToReceive.ToString()));
                                                             Invoke((MethodInvoker)(() => kop5Available.Value = coinsAvailable[typeOfCoinToReceive])); }

                        else if (3 == typeOfCoinToReceive) { Invoke((MethodInvoker)(() => kop10Received.Text = countOfCoinsToReceive.ToString()));
                                                             Invoke((MethodInvoker)(() => kop10Available.Value = coinsAvailable[typeOfCoinToReceive])); }

                        else if (4 == typeOfCoinToReceive) { Invoke((MethodInvoker)(() => kop25Received.Text = countOfCoinsToReceive.ToString()));
                                                             Invoke((MethodInvoker)(() => kop25Available.Value = coinsAvailable[typeOfCoinToReceive])); }

                        else if (5 == typeOfCoinToReceive) { Invoke((MethodInvoker)(() => kop50Received.Text = countOfCoinsToReceive.ToString())); 
                                                             Invoke((MethodInvoker)(() => kop50Available.Value = coinsAvailable[typeOfCoinToReceive])); }

                        else if (6 == typeOfCoinToReceive) { Invoke((MethodInvoker)(() => hrn1Received.Text = countOfCoinsToReceive.ToString())); 
                                                             Invoke((MethodInvoker)(() => hrn1Available.Value = coinsAvailable[typeOfCoinToReceive])); }
                    }

                    Invoke((MethodInvoker)(() => Output.Text = outText));

                    coinInputed = false;
                }

                CommonToNull();

                Thread.Sleep(100);
            }
        }

        void WhatDoWeWantToReceive()
        {
            if (kop1Out.Checked) { typeOfCoinToReceive = 0; }
            else if (kop2Out.Checked) { typeOfCoinToReceive = 1; }
            else if (kop5Out.Checked) { typeOfCoinToReceive = 2; }
            else if (kop10Out.Checked) { typeOfCoinToReceive = 3; }
            else if (kop25Out.Checked) { typeOfCoinToReceive = 4; }
            else if (kop50Out.Checked) { typeOfCoinToReceive = 5; }
            else if (hrn1Out.Checked) { typeOfCoinToReceive = 6; }
            else { typeOfCoinToReceive = -1; }
        }

        void WhatCoinInputed()
        {
            if (kop1In.Checked) { typeOfNewCoin = 0; }
            else if (kop2In.Checked) { typeOfNewCoin = 1; }
            else if (kop5In.Checked) { typeOfNewCoin = 2; }
            else if (kop10In.Checked) { typeOfNewCoin = 3; }
            else if (kop25In.Checked) { typeOfNewCoin = 4; }
            else if (kop50In.Checked) { typeOfNewCoin = 5; }
            else if (hrn1In.Checked) { typeOfNewCoin = 6; }
            else { typeOfNewCoin = -1; }
        }

        static void TS(ref int left, ref int right)
        {
            lock (l)
            {
                left = right;
                right = 1;
            }
        }

        static void CommonToNull()
        {
            lock (l)
            {
                common = 0;
            }
        }

        private void inCoin_Click(object sender, EventArgs e)
        {
            coinEntered = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            A.Abort();

            B.Abort();
        }

        
    }
}
