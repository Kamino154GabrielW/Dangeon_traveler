using System;
using System.Diagnostics.Eventing.Reader;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace dangeon_traveler
{
    public partial class MainWindow : Window
    {
        private Character player;
        private Character enemy;

        private static readonly Random random = new Random();

        public static class GlobalData
        {
            public static bool IsPlayerTurn { get; set; } = true;
            public static int player_active_iniciative { get; set; } = 1;
        }

        public class Character
        {
            public int MaxHP { get; set; }
            public int HP { get; set; }
            public int DMG { get; set; }
            public int Defense { get; set; }
            public int DFP { get; set; }
            public int Shield { get; set; }

            public int iniciative { get; set; }

            public string C_Name { get; set; }

            public Character(int hp, int dmg, int defense, int dfp, int inic, string n)
            {
                MaxHP = hp;
                HP = hp;
                DMG = dmg;
                Defense = defense;
                DFP = dfp;
                Shield = 0;
                iniciative = inic;
                C_Name = n;
            }
            public void create_new_enemie(string enemy_tier , double level)
            {
                
                int New_Enemy_index = random.Next(1, 5 );
                if (New_Enemy_index<3)
                {
                    MaxHP = Convert.ToInt32(Math.Floor(45 * level));
                    HP = Convert.ToInt32(Math.Floor(45 * level));
                    DMG = Convert.ToInt32(Math.Floor(5 * level)) ;
                    Defense = Convert.ToInt32(Math.Floor(1 * level));
                    DFP = 0;
                    Shield = 0;
                    iniciative = 1;
                    C_Name = "Skeleton";
                }
                else if (New_Enemy_index == 3)
                {
                    MaxHP = 60;
                    HP = 60;
                    DMG = 4;
                    Defense = 2;
                    DFP = 0;
                    Shield = 3;
                    iniciative = 1;
                    C_Name = "SkeletonKnight";
                }
                else
                {
                    MaxHP = 30;
                    HP = 30;
                    DMG = 6;
                    Defense = 1;
                    DFP = 0;
                    Shield = 0;
                    iniciative = 2;
                    C_Name = "SkeletonArcher";

                }
                /*
                  MaxHP = 70;
                    HP = 70;
                    DMG = 6;
                    Defense = 0;
                    DFP = 0;
                    Shield = 0;
                    iniciative = 1;
                    C_Name = "Slime";

                """ he will have higher chanse to do nothing """

                ------------------------

                 MaxHP = 100;
                    HP = 35;
                    DMG = 4;
                    Defense = 1;
                    DFP = 0;
                    Shield = 0;
                    iniciative = 2;
                    C_Name = "lower vampire";

                """ every turn he regains 1hp every atack he regains (50%of atack) HP  """

                ------------------------

                 MaxHP = 50;
                    HP = 35;
                    DMG = 1;
                    Defense = 1;
                    DFP = 0;
                    Shield = 0;
                    iniciative = 1;
                    C_Name = "Skelton paladin";

                """ when he skips turn he charges his atack (+1 dmg) i ma dodatkow alternatwny atak  zadajoncy 2xDmg ale potem ucinajonc dmg/2 """
                 
                 */
            }

        }

        public MainWindow()
        {
            InitializeComponent();
            StartBattle();
        }

        public void StartBattle()
        {
           
            player = new Character(50, 5, 2, 0, 1, "knight");
            enemy = new Character(0, 0, 0, 0, 0, "enemie");
            enemy.create_new_enemie("",1);
            PlayerTurn();
        }

        public void PlayerTurn()
        { 
            GlobalData.IsPlayerTurn = true;
            GlobalData.player_active_iniciative = player.iniciative;
           
            if (player.HP <= 0)
            {
                Game_over("");
            }
            player.Shield = 0;
            UpdateGui();
        }

        public async Task EnemieTurn()
        {
            GlobalData.IsPlayerTurn = false;
            wnik_akcji_label.Text = "";
            

            if (enemy.HP <= 0)
            {
                Game_over("enemy");
                return; // Stop execution if game is over
            }

            UpdateGui();
           

            wnik_akcji_label.Text = ""; 

            for (int i = 0; i < enemy.iniciative; i++)
            {
                await Task.Delay(1000);
                enemy.DFP += enemy.Defense;

                int enemyMove = random.Next(1, 7 + (enemy.DFP / 2));

                if (enemyMove <= 4) 
                {
                    int damageDealt = Math.Max(0, enemy.DMG - player.Shield);
                    player.HP = Math.Max(0, player.HP - damageDealt);

                    wnik_akcji_label.Text += $"Enemy attacks you for {damageDealt} DMG\n";
                }
                else if (enemyMove == 5) 
                {
                    wnik_akcji_label.Text += "Enemy does nothing\n";
                }
                else 
                {
                    enemy.Shield = enemy.DFP;
                    enemy.DFP = 0;
                    wnik_akcji_label.Text += $"Enemy shields himself \n Next turn deal -{enemy.Shield} dmg\n";
                }

                UpdateGui();
               
            }
            await Task.Delay(2000);
            PlayerTurn();
        }

        public void Player_Atack_Button(object sender, RoutedEventArgs e)
        {
            if (GlobalData.IsPlayerTurn)
            {
                player.DFP += player.Defense;

                int damageDealt = Math.Max(0, player.DMG - enemy.Shield);
                enemy.HP -= damageDealt;

                if (enemy.HP < 0) enemy.HP = 0;

                wnik_akcji_label.Text = $"You attacked enemy for {damageDealt} DMG";

                player_iniciative_chek();
            }
        }

        public void Player_Defend_Button(object sender, RoutedEventArgs e)
        {
            if (GlobalData.IsPlayerTurn)
            {
                player.Shield = player.DFP;
                player.DFP = 0;

                player_iniciative_chek();
            }
        }

        public void Player_Skip_Button(object sender, RoutedEventArgs e)
        {
            if (GlobalData.IsPlayerTurn)
            {
                player.DFP += player.Defense * 2;

                player_iniciative_chek();
            }
        }

        public void UpdateGui()
        {
            Enemie_HP_label.Content = $"HP: {enemy.HP}/{enemy.MaxHP}";
            Enemie_DFP_label.Content = $"DFP: {enemy.DFP}";
            player_HP_label.Content = $"HP: {player.HP}/{player.MaxHP}";
            player_DFP_label.Content = $"DFP: {player.DFP}";
            Enemie_Shield_label.Content = $"shild:{enemy.Shield}";
            Enemie_name_label.Content = enemy.C_Name;
            Enemie_iniciative_label.Content = $"iniciative:{enemy.iniciative}";

            player_iniciative_label.Content = $"iniciative:{player.iniciative}";
        }

        private void Atack_button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
                var accual_DMG = 0;
                if (player.DMG - enemy.DFP > 0)
                {
                    accual_DMG = player.DMG - enemy.DFP;
                }
                else { accual_DMG = 0; }
                wnik_akcji_label.Text = $"Atack enemie for {player.DMG} dmg \n if he Defends: {accual_DMG} dmg";
            
        }

        private void Defend_button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
         
           
                wnik_akcji_label.Text = $"Shild yourself from {player.DFP}dmg \n if he atacks ";
            
        }

        private void Skip_button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
                wnik_akcji_label.Text = "skip your turn";
            
        }
        private void Game_over(String side)
        {
            if (side == "enemy")
            {
                wnik_akcji_label.Text = "YOU WIN";
            }
            else
            {
                wnik_akcji_label.Text = "YOU LOSE";

            }
            Thread.Sleep(4000);
            player.HP = player.MaxHP;
            player.DFP = 0;
            enemy.HP = enemy.MaxHP;
            enemy.DFP = 0;

        }
        private void player_iniciative_chek()
        {
            GlobalData.player_active_iniciative -= 1;
            UpdateGui();
            if  ( GlobalData.player_active_iniciative < 1)
            {
                EnemieTurn();
            }
        }
        

    }
}
