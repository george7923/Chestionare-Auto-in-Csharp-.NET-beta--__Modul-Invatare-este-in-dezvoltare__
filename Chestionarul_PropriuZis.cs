﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Chestionar_Auto
{
    public partial class Chestionarul_PropriuZis : Form
    {
        private List<Button> ButoaneleMele = new List<Button>();
        private List<Intrebare> ListaIntrebari = new List<Intrebare>();
        private MySqlConnection a = new MySqlConnection();
        private List<Intrebare> IntrebariUmplute = new List<Intrebare>();
        private List<Raspuns> RaspunsurileExistente = new List<Raspuns>();
        private Intrebare intrebareaSelectata;
        private int contor = 1, raspunsuriCorecte = 0, raspunsuriGresite = 0;
        private bool IsSelectedA = false;
        private bool IsSelectedB = false;
        private bool IsSelectedC = false;
        private List<bool> Selectate = new List<bool>();
        private bool IsSelectedSUBMIT = false;
        private bool A_RaspunsCorect;
        private bool prima_intrebare = true;
        private Timer countdownTimer;
        private int remainingSeconds = 30 * 60;
        private bool CheckTheCorrect = false;
        public Chestionarul_PropriuZis()
        {
            
            InitializeComponent();
            ButoaneleMele.Add(buttonA);
            ButoaneleMele.Add(buttonB);
            ButoaneleMele.Add(buttonC);
            Selectate.Add(IsSelectedA);
            Selectate.Add(IsSelectedB);
            Selectate.Add(IsSelectedC);

            string query = "SELECT I.ID, I.Intrebare, I.Categoria, R.Raspuns, R.EsteCorect, I.Imagine " +
                           "FROM INTREBARI I " +
                           "JOIN RASPUNSURI R ON I.ID = R.Intrebare_ID " +
                           "ORDER BY I.ID";
            BazaDeDate bd = new BazaDeDate();
            bd.OpenConnection();
            a = bd.GetConnection();
            OrganizareIntrebari(ListaIntrebari, query, ButoaneleMele);
            buttonSTART.Text = "SUBMIT";

            GestorChestionar();
            AfisareIntrebari();
            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; 
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();



        }
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {

            remainingSeconds--;


            TimeSpan time = TimeSpan.FromSeconds(remainingSeconds);
            label5.Text = time.ToString(@"mm\:ss");


            if (remainingSeconds <= 0)
            {
                countdownTimer.Stop();
                MessageBox.Show("Timpul a expirat!");
            }
        }
        private void InsertHighScore(string Calificativ)
        {
            int ID = -1;
            string Username = ManagementVariabileGlobale.GetUserName();
            
            string query = "SELECT ID FROM cont WHERE username = '"+Username+"';";
            using (MySqlCommand command = new MySqlCommand(query, a))
            {


                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        ID = reader.GetInt32(0);
                        
                    }
                }
            }
            MessageBox.Show(Username+" "+ID.ToString());
            if (ID != -1)
            {
                string query1 = "INSERT INTO highscore (ID_Cont, Timpul_Ramas, IntrebarileRaspunse, IntrebarileGresite, IntrebarileCorecte, Calificativ) VALUES (@IdCont, @TimpRamas, @IntrRaspunse, @Gresite, @Corecte, @Calificativ)";
                using (MySqlCommand comanda = new MySqlCommand(query1, a))
                {
                    
                    //@IdCont, @TimpRamas, @IntrRaspunse, @Gresite, @Corecte, @Calificativ
                    comanda.Parameters.AddWithValue("@IdCont", ID);
                    comanda.Parameters.AddWithValue("@TimpRamas", label5.Text);
                    comanda.Parameters.AddWithValue("@IntrRaspunse", contor);
                    comanda.Parameters.AddWithValue("@Gresite", raspunsuriGresite);
                    comanda.Parameters.AddWithValue("@Corecte", raspunsuriCorecte);
                    comanda.Parameters.AddWithValue("@Calificativ", Calificativ);





                    int randuriAfectate = comanda.ExecuteNonQuery();


                    if (randuriAfectate > 0)
                    {
                        Console.WriteLine($"Datele au fost inserate cu succes. Numărul de rânduri afectate: {randuriAfectate}");


                    }
                    else
                    {
                        Console.WriteLine("Nu s-au inserat date.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Ceva nu a mers bine!");
            }
        }
        public void AfisareIntrebari()
        {
            for(int  i = 0; i < ListaIntrebari.Count; i++)
            {
                Console.WriteLine("INTREBAREA NR: " + i.ToString() + ": " + ListaIntrebari[i].intrebare);
                Console.WriteLine(ListaIntrebari[i].Raspuns1.Text + " " + ListaIntrebari[i].Raspuns1.Corect.ToString());
                Console.WriteLine(ListaIntrebari[i].Raspuns2.Text + " " + ListaIntrebari[i].Raspuns2.Corect.ToString());
                Console.WriteLine(ListaIntrebari[i].Raspuns3.Text + " " + ListaIntrebari[i].Raspuns3.Corect.ToString());
            }
        }

        private void GestorChestionar()
        {
            
                label6.Text = contor.ToString();
                LabelRaspGresite.Text = raspunsuriGresite.ToString();
                LabelRaspCorecte.Text = raspunsuriCorecte.ToString();

                //MessageBox.Show(intr.intrebare );
                Random r = new Random();
                int indexAleatoriu = r.Next(0, ListaIntrebari.Count);
                Intrebare INTREBAREA = ListaIntrebari[indexAleatoriu];
                RaspunsurileExistente.Add(INTREBAREA.Raspuns1);
                RaspunsurileExistente.Add(INTREBAREA.Raspuns2);
                RaspunsurileExistente.Add(INTREBAREA.Raspuns3);
                label_INTREBARE.Text = INTREBAREA.intrebare;
                label_R1.Text = INTREBAREA.Raspuns1.Text;
                label_R2.Text = INTREBAREA.Raspuns2.Text;
                label_R3.Text = INTREBAREA.Raspuns3.Text;
                pictureBox1.Image = INTREBAREA.img;
                prima_intrebare = false;
           
                
               
            
            
          
        }
        private void button4_Click(object sender, EventArgs e)
        {
            IsSelectedSUBMIT = true;
            if (ManagementVariabileGlobale.GetMod() == "EXAMEN")
            {
                if (prima_intrebare)
                {
                    Punctajul(CheckCorectitudine(RaspunsurileExistente));
                    RaspunsurileExistente.Clear();
                    GestorChestionar();
                    IsSelectedSUBMIT = false;
                    DeselecteazaButoanele(Selectate);
                }
                else
                {
                    while (IsSelectedSUBMIT)
                    {

                        Punctajul(CheckCorectitudine(RaspunsurileExistente));
                        RaspunsurileExistente.Clear();
                        DeselecteazaButoanele(Selectate);
                        if (raspunsuriGresite <= 4)
                        {
                            GestorChestionar();
                            IsSelectedSUBMIT = false;
                            
                        }
                        if (raspunsuriGresite > 4 || (raspunsuriCorecte >= 22 && contor > 26))
                        {

                            if (raspunsuriGresite > 4 || label5.Text == "00:00")
                            {
                                PICAT p = new PICAT();
                                InsertHighScore("RESPINS");
                                p.Show();
                                this.Hide();
                                break;
                            }
                            if (raspunsuriCorecte >= 22 && contor > 26)
                            {
                                ADMIS a = new ADMIS();
                                InsertHighScore("ADMIS");
                                a.Show();
                                this.Hide();
                                break;
                            }
                        }
                    }
                }
                
            }
            else
            {

                
                    if (prima_intrebare)
                    {
                        Punctajul(CheckCorectitudine(RaspunsurileExistente));
                        RaspunsurileExistente.Clear();
                        GestorChestionar();
                        IsSelectedSUBMIT = false;
                        DeselecteazaButoanele(Selectate);
                    AfiseazaRaspunsurileCorecte(RaspunsurileExistente);
                    CheckTheCorrect = false;

                }
                    else
                    {
                    int i = 0;
                    
                        while (IsSelectedSUBMIT)
                        {
                        i++;
                            if (i == 0)
                            {
                                Punctajul(CheckCorectitudine(RaspunsurileExistente));
                                RaspunsurileExistente.Clear();
                                DeselecteazaButoanele(Selectate);
                                if (raspunsuriGresite <= 4)
                                {
                                GestorChestionar();
                                IsSelectedSUBMIT = false;

                                }
                                if (raspunsuriGresite > 4 || (raspunsuriCorecte >= 22 && contor > 26))
                                {

                                    if (raspunsuriGresite > 4 || label5.Text == "00:00")
                                    {
                                    PICAT p = new PICAT();
                                    p.Show();
                                    this.Hide();

                                    }
                                    if (raspunsuriCorecte >= 22 && contor > 26)
                                    {
                                    ADMIS a = new ADMIS();
                                    a.Show();
                                    this.Hide();

                                    }
                            }
                        }
                            else
                            {
                            AfiseazaRaspunsurileCorecte(RaspunsurileExistente);
                            }
                        }
                    }
                    
                

            }
        }
        private void AfiseazaRaspunsurileCorecte(List<Raspuns> R)
        {
            List<Button> Buttons = new List<Button>() { buttonA, buttonB, buttonC };


            for (int i = 0; i < R.Count; i++)
            {
                if (R[i].Corect)
                {
                    Buttons[i].BackColor = Color.Green;
                }
                else
                {
                    Buttons[i].BackColor = Color.Red;
                }
            }
            
        }

        private void DeselecteazaButoanele(List<bool> button)
        {
            for (int i = 0; i < button.Count; i++)
            {
                button[i] = false;
            }

            IsSelectedA = false;
            IsSelectedB = false;
            IsSelectedC = false;
            buttonA.BackColor = Color.White;
            buttonB.BackColor = Color.White;
            buttonC.BackColor = Color.White;
        }
        private void Punctajul(bool A_RaspunsCorect)
        {
            contor++;
            if (!A_RaspunsCorect)
            {
                raspunsuriGresite++;
            }
            else
            {
                raspunsuriCorecte++;
                A_RaspunsCorect = false;
            }
        }
        private bool CheckCorectitudine(List<Raspuns> R)
        {

            List<bool> Selected = new List<bool>() { IsSelectedA, IsSelectedB, IsSelectedC };


            for(int i = 0; i < R.Count; i++)
            {
                if ((Selected[i] != R[i].Corect)||((Selected[i] == R[i].Corect && (R[i].Corect == false /* a fost = false*/)))) 
                {
                    return false;
                }
            }
            return true;


        }
        private void SeteazaBackgroundButon()
        {
            
        }
        private void buttonA_Click_1(object sender, EventArgs e)
        {
            IsSelectedA = !IsSelectedA;
            if (IsSelectedA)
            {
                buttonA.BackColor = Color.DarkOrange;
            }
            else
            {
                buttonA.BackColor = Color.White;
            }
        }

        private void buttonB_Click_1(object sender, EventArgs e)
        {
            IsSelectedB = !IsSelectedB;
            if (IsSelectedB)
            {
                buttonB.BackColor = Color.DarkOrange;
            }
            else
            {
                buttonB.BackColor = Color.White;
            }
        }

        private void buttonC_Click_1(object sender, EventArgs e)
        {
            IsSelectedC = !IsSelectedC;
            if (IsSelectedC)
            {
                buttonC.BackColor = Color.DarkOrange;
            }
            else
            {
                buttonC.BackColor = Color.White;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void OrganizareIntrebari(List<Intrebare> ListaIntrebari, string query, List<Button> B)
        {
            using (MySqlCommand command = new MySqlCommand(query, a))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    int i = 0;
                    MessageBox.Show(B.Count.ToString());
                    List<Raspuns> R = new List<Raspuns>();
                    while (reader.Read())
                    {
                        
                        string ID_String = reader.GetInt32("ID").ToString();
                        int ID = Convert.ToInt32(ID_String);
                        string intrebare = reader.GetString("Intrebare"); 
                        string raspuns = reader.GetString("Raspuns"); 
                        bool esteCorect = reader.GetBoolean("EsteCorect"); 
                        string categorie = reader.GetString("Categoria"); 

                        Console.WriteLine("NU E IN LISTA: " + intrebare);
                        

                        byte[] imageBytes = null;
                        Image imagine = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Imagine")))
                        {
                            imageBytes = (byte[])reader["Imagine"];
                        }

                        if (imageBytes != null)
                        {
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                imagine = Image.FromStream(ms);
                            }
                        }

                        if (i % 2 == 0&&i!=0)
                        {
                            Raspuns R_R = new Raspuns(raspuns, esteCorect,B[i]);
                            R.Add(R_R);
                            Intrebare a = new Intrebare(intrebare, R[0], R[1], R[2], categorie, false, ID, imagine);
                            ListaIntrebari.Add(a);
                            for(int j = 0; j < R.Count; j++)
                            {
                                
                                Console.WriteLine(R.Count);
                            }
                            R.Clear();
                            i = 0;
                            
                        }
                        else
                        {
                            Raspuns R_R = new Raspuns(raspuns, esteCorect, B[i]);
                            R.Add(R_R);
                            Console.WriteLine(raspuns);
                            i++;

                        }



                    }
                }
            }

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
