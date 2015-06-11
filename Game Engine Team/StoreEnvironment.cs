using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python_Team;
using Controls;
using Game_Engine_Team.Equipment;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework.Input;

namespace Game_Engine_Team
{
    /// <summary>
    /// 
    /// </summary>
    public class StoreEnvironment : Screen
    {
        public bool display { get; set; }

        // 
        private int verticalPadding = 2;
        private SpriteFont monospace;
        private Rectangle bounds;
        private int horizontal;
        private int vertical;

        private List<Button> buttons = new List<Button>();
        private List<Button> equipButtons = new List<Button>();
        private List<Button> monsterButtons = new List<Button>();

        private List<Equipment.Equipment> items = new List<Equipment.Equipment>();
        private List<Equipment.Equipment> setOfItems = new List<Equipment.Equipment>();
        private List<Equipment.Equipment> selectedItems = new List<Equipment.Equipment>();

        private List<IEntity> entities = new List<IEntity>();
        private List<IEntity> setOfEntities = new List<IEntity>();
        private List<IEntity> selectedEntities = new List<IEntity>();

        private int SelectedItemSet = 0;
        private int SelectedMonsterSet = 0;

        private int currentCost = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public StoreEnvironment( MainController game )
            : base( game )
        {
            display = true;
            int width = (int)( Screen.Width * .8 );
            int height = (int)( Screen.Height * .8 );
            int x = ( Screen.Width - width ) / 2;
            int y = ( Screen.Height - height ) / 2;

            bounds = new Rectangle( x, y, width, height );
            monospace = Game.Content.Load<SpriteFont>( "Fonts/MonoSpace" );

            buttons.Add( new Button( monospace, "^", 0, 0, 40 ) );
            buttons.Last().On_Button_Click += new Button.ButtonClick( UpArrow );
            buttons.Add( new Button( monospace, "v", 0, 0, 40 ) );
            buttons.Last().On_Button_Click += new Button.ButtonClick( DownArrow );
            buttons.Add( new Button( monospace, "^", 0, 0, 40 ) );
            buttons.Last().On_Button_Click += new Button.ButtonClick( UpArrow2 );
            buttons.Add( new Button( monospace, "v", 0, 0, 40 ) );
            buttons.Last().On_Button_Click += new Button.ButtonClick( DownArrow2 );
            buttons.Add( new Button( monospace, "Buy Items", 0, 0, 100 ) );
            buttons.Last().On_Button_Click += new Button.ButtonClick( SubmitClick );
            buttons.Add( new Button( monospace, "Cancel", 0, 0, 100 ) );
            buttons.Last().On_Button_Click += new Button.ButtonClick( Cancel );

            equipButtons = new List<Button>() {
                new Button( monospace ),
                new Button( monospace ),
                new Button( monospace ),
                new Button( monospace )
            };

            monsterButtons = new List<Button>() {
                new Button( monospace ),
                new Button( monospace ),
                new Button( monospace ),
                new Button( monospace )
            };

            foreach ( Button button in monsterButtons )
                button.On_Select_Button_Click += new Button.SelectButtonClick( MonsterPressed );

            foreach ( Button button in equipButtons )
                button.On_Select_Button_Click += new Button.SelectButtonClick( EquipmentPressed );

            items = new List<Equipment.Equipment>();
            foreach ( var weapon in EnumUtil.GetValues<WeaponType>() )
            {
                if ( !weapon.ToString().Equals( "None" ) )
                    items.Add( Equipment.Equipment.Database.Get( weapon ) );
            }
            foreach ( var helmet in EnumUtil.GetValues<HeadType>() )
            {
                if ( !helmet.ToString().Equals( "None" ) )
                    items.Add( Equipment.Equipment.Database.Get( helmet ) );
            }
            foreach ( var shirt in EnumUtil.GetValues<ChestType>() )
            {
                if ( !shirt.ToString().Equals( "None" ) )
                    items.Add( Equipment.Equipment.Database.Get( shirt ) );
            }
            foreach ( var pants in EnumUtil.GetValues<LegsType>() )
            {
                if ( !pants.ToString().Equals( "None" ) )
                    items.Add( Equipment.Equipment.Database.Get( pants ) );
            }

            entities = new List<IEntity>();
            foreach ( Actors.EnemyType enemy in (Actors.EnemyType[])Enum.GetValues( typeof( Actors.EnemyType ) ) )
            {
                entities.Add( Actors.Enemy.Database.Get( enemy, new Dungeon() ) );
            }
            /*foreach ( TrapType trap in (TrapType[])Enum.GetValues( typeof( Trap ) ) )
            {
                entities.Add( Trap.Database.Get( trap, new Dungeon() ) );
            }*/

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        private void LoadContent( ContentManager content )
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Update( gameTime );
            //Find centre of the screen
            if ( horizontal != Screen.Width / 2 )
                horizontal = Screen.Width / 2;
            vertical = Screen.Height / 2;

            //Update Up and Down buttons
            int width = buttons.ElementAt( 3 ).Location.Width;
            int height = buttons[ 0 ].Location.Height;

            int startX = width * 4;
            int startY = vertical + 6 * height - verticalPadding;

            for ( int i = 0; i < 2; i++ )
            {
                buttons.ElementAt( i ).Location = new Rectangle( startX + 75, startY - 100, width, height );
                buttons.ElementAt( i ).Text = buttons.ElementAt( i ).Text;
                startX += width + verticalPadding;
            }

            //Update Up and Down buttons
            width = buttons.ElementAt( 3 ).Location.Width;
            startX = horizontal + width * 4;
            startY = vertical + 6 * height - verticalPadding;

            for ( int i = 2; i < 4; i++ )
            {
                buttons.ElementAt( i ).Location = new Rectangle( startX + 75, startY - 100, width, height );
                buttons.ElementAt( i ).Text = buttons.ElementAt( i ).Text;
                startX += width + verticalPadding;
            }

            //Update Submit and Cancel buttons
            width = buttons.ElementAt( 4 ).Location.Width;
            startX = horizontal - width;
            startY = vertical + 6 * height - verticalPadding;

            for ( int i = 4; i < buttons.Count; i++ )
            {
                buttons.ElementAt( i ).Location = new Rectangle( startX + 100, startY - 100, width, height );
                buttons.ElementAt( i ).Text = buttons.ElementAt( i ).Text;
                startX += width + verticalPadding;
            }
            int startx = 20;
            int starty = 20;

            setOfItems = items.GetRange( SelectedItemSet, 4 );
            setOfEntities = entities.GetRange( SelectedMonsterSet, 4 );

            //Update buttons for base selection
            width = buttons.ElementAt( 1 ).Location.Width;
            startX = startx;
            startY = starty;
            for ( int i = 0; i < equipButtons.Count; i++ )
            {

                equipButtons.ElementAt( i ).Location = new Rectangle( startX + 3, startY + 3, (int)( width * 4 ), height * 5 );
                equipButtons.ElementAt( i ).Text = "" + setOfItems.ElementAt( i ).Name + "\n" +
                                                "Defence:" + setOfItems.ElementAt( i ).Defence + "\n" +
                                                //"Dexterity:" + setOfItems.ElementAt( i ).Dexterity + "\n" +
                                                "Damage:" + setOfItems.ElementAt( i ).Damage + "\n" +
                                                "Total Cost: $" + setOfItems.ElementAt( i ).GoldCost;
                startX += (int)( width * 4 ) + verticalPadding;
                if ( i % 2 == 1 )
                {
                    startY += height * 5 + verticalPadding;
                    startX = startx;
                }
            }
            startY = starty;
            for ( int i = 0; i < monsterButtons.Count; i++ )
            {

                monsterButtons.ElementAt( i ).Location = new Rectangle( startX + 350, startY + 3, (int)( width * 4 ), height * 5 );
                monsterButtons.ElementAt( i ).Text = "Name:" + setOfEntities.ElementAt( i ) + "\n" +
                                                "Cost: $" + ( setOfEntities.ElementAt( i ) as Actors.Enemy ).GoldCost;
                startX += (int)( width * 4 ) + verticalPadding;
                if ( i % 2 == 1 )
                {
                    startY += height * 5 + verticalPadding;
                    startX = startx;
                }
            }



            //Update control buttons
            foreach ( Button btn in buttons )
                btn.Update();
            //Update Item selection buttons
            foreach ( Button itembtn in equipButtons )
                itembtn.Update();
            //Update Monster selection buttons
            foreach ( Button emembtn in monsterButtons )
                emembtn.Update();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw( Canvas canvas )
        {
            //Draw each button for screen control
            foreach ( Button btn in buttons )
                btn.Draw( canvas );

            //Draw each Item selection button
            foreach ( Button itembtn in equipButtons )
                itembtn.Draw( canvas );

            //Draw each Monster selection button
            foreach ( Button monstbtn in monsterButtons )
                monstbtn.Draw( canvas );

            canvas.DrawString( monospace, "Your Balence: " + User.Instance.Balance,
                new Vector2( bounds.X - 50, bounds.Y * 6 + verticalPadding ),
                Color.White );
            canvas.DrawString( monospace, "Current Cost: " + currentCost,
                new Vector2( bounds.X - 50, bounds.Y * 6 + verticalPadding + 15 ),
                Color.White );
            canvas.DrawString( monospace, "Selected Items: ",
                new Vector2( bounds.X - 50, bounds.Y * 6 + verticalPadding + 30 ),
                Color.White );
            canvas.DrawString( monospace, "Selected Enemies: ",
                new Vector2( bounds.X - 50, bounds.Y * 6 + verticalPadding + 135 ),
                Color.White );
            canvas.DrawString( monospace, "Equipment ",
                new Vector2( bounds.X, 0 ),
                Color.White );
            canvas.DrawString( monospace, "Monsters ",
                new Vector2( bounds.X + 350, 0 ),
                Color.White );

            if ( selectedItems.Count > 0 )
            {
                string s = "";
                int max = 60;
                for ( int i = 0; i < selectedItems.Count; i++ )
                {
                    if ( s.Length + ( ", " + selectedItems.ElementAt( 0 ).Name ).Length > 200 )
                    {
                        s += "...";
                        break;
                    }
                    else if ( s.Length + ( ", " + selectedItems.ElementAt( 0 ).Name ).Length > max )
                    {
                        max += 60;
                        s += "\n";
                    }
                    s += selectedItems.ElementAt( i ).Name + ", ";
                }
                canvas.DrawString( monospace, s, new Vector2( bounds.X - 50, bounds.Y * 6 + verticalPadding + 45 ), Color.White );
            }
            if ( selectedEntities.Count > 0 )
            {
                string s = "";
                int max = 60;
                for ( int i = 0; i < selectedEntities.Count; i++ )
                {
                    if ( s.Length + ( ", " + selectedEntities.ElementAt( i ) ).Length > 120 )
                    {
                        s += "...";
                        break;
                    }
                    else if ( s.Length + ( ", " + selectedEntities.ElementAt( i ) ).Length > max )
                    {
                        max += 60;
                        s += "\n";
                    }
                    s += selectedEntities.ElementAt( i ) + ", ";
                }
                canvas.DrawString( monospace, s, new Vector2( bounds.X - 50, bounds.Y * 6 + verticalPadding + 150 ), Color.White );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DownArrow()
        {
            if ( SelectedItemSet < items.Count - 4 )
            {
                SelectedItemSet++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpArrow()
        {
            if ( SelectedItemSet > 0 )
            {
                SelectedItemSet--;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DownArrow2()
        {
            if ( SelectedMonsterSet < entities.Count - 4 )
            {
                SelectedMonsterSet++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpArrow2()
        {
            if ( SelectedMonsterSet > 0 )
            {
                SelectedMonsterSet--;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SubmitClick()
        {
            if ( User.Instance.Balance >= currentCost )
            {
                User.Instance.Balance -= currentCost;
                Inventory myInventory = User.Instance.CurrentCharacter.InventoryItems;

                foreach ( Equipment.Equipment equip in selectedItems )
                {
                    if ( equip is Weapon )
                        myInventory[ ( equip as Weapon ).MyType ]++;
                    else if ( equip is Helmet )
                        myInventory[ ( equip as Helmet ).MyType ]++;
                    else if ( equip is Shirt )
                        myInventory[ ( equip as Shirt ).MyType ]++;
                    else if ( equip is Leggings )
                        myInventory[ ( equip as Leggings ).MyType ]++;
                }
                foreach ( IEntity entity in selectedEntities )
                {
                    if ( entity is Actors.Enemy )
                    {
                        myInventory[ ( entity as Actors.Enemy ).MyType.Value ]++;
                    }
                    else if ( entity is Trap )
                    {
                        myInventory[ ( entity as Trap ).MyType ]++;
                    }
                }

                User.Instance.CurrentCharacter.Save();
                Game.Back();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            selectedItems.RemoveRange( 0, selectedItems.Count );
            selectedEntities.RemoveRange( 0, selectedEntities.Count );
            currentCost = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public void EquipmentPressed( Object sender )
        {
            int i = equipButtons.IndexOf( (Button)sender );
            if ( !selectedItems.Contains( setOfItems.ElementAt( i ) ) )
            {
                selectedItems.Add( setOfItems.ElementAt( i ) );
                currentCost += setOfItems.ElementAt( i ).GoldCost;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public void MonsterPressed( Object sender )
        {
            int i = monsterButtons.IndexOf( (Button)sender );
            selectedEntities.Add( setOfEntities.ElementAt( i ) );
            currentCost += ( setOfEntities.ElementAt( i ) as Actors.Enemy ).GoldCost;
        }
    }
}
