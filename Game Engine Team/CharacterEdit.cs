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
using System.Net;

namespace Game_Engine_Team
{
    /// <summary>
    /// Represents a piece of equipment at a particular location,
    /// which was necessary for drag and drop functionality.
    /// </summary>
    public class equipment
    {
        public Equipment.Equipment equip;
        public Point location;

        public equipment( Equipment.Equipment e, Point p )
        {
            equip = e;
            location = p;
        }
    }

    /// <summary>
    /// The character editor that can be used to modify a characters
    /// equipment and stats, as well as view what equipment a user has.
    /// 
    /// Created by: Corey Dries
    /// </summary>
    public class CharacterEdit : Screen
    {
        /// <summary>
        /// A common denominator of all the possible screen widths and heights,
        /// this is used to place objects relative to the size of the screen
        /// rather than at an absolute position
        /// </summary>
        private const int DENOM = 16;

        private const int WRAP_LENGTH = 30;
        private const int DRAWING_OFFSET = 20;

        /// <summary>
        /// Clickable buttons for stats, saving and quitting,
        /// and static buttons for equipment places
        /// </summary>
        private LinkedList<Button> buttons;
        private LinkedList<Button> staticButtons;

        // The list of equipment displayed down the left side of the screen
        private List<equipment> equips = new List<equipment>();

        // The currently selected equipment that is used for the detail view in the bottom left of the screen
        private equipment selected;

        // The currently clicked equipment ie. when you click on something, before you put it in a slot
        private Equipment.Equipment clicked;

        // These are the equipment that are getting equipped, ie. the things that got dragged into the boxes
        private equipment weap;
        private equipment helm;
        private equipment shirt;
        private equipment legs;

        private Texture2D background;

        private SpriteFont monospace;


        private Stats Stats;

        // Update this when the user changes his equipment. Just stores enums.
        private EquipmentSet Gear;

        private int EquipEXP;

        private int HP_Ratio = 1;
        private int ATK_Ratio = 1;
        //private int DEX_Ratio = 1;

        public ContentManager Content { get; private set; }


        private Character character;


        public CharacterEdit( MainController game, Character _character )
            : base( game )
        {
            Content = game.Content;

            character = _character;

            Stats = character.Stats;
            Gear = character.Gear;
            EquipEXP = character.Experience;

            buttons = new LinkedList<Button>();
            staticButtons = new LinkedList<Button>();
            LoadContent( Content );

            weap = new equipment( Equipment.Equipment.Database.Get( Gear.Weapon ),
                new Point( staticButtons.ElementAt( 3 ).Location.Center.X - 16, staticButtons.ElementAt( 3 ).Location.Center.Y - 16 ) );
            EquipEXP -= Equipment.Equipment.Database.Get( Gear.Weapon ).ExpCost;

            helm = new equipment( Equipment.Equipment.Database.Get( Gear.Head ),
                new Point( staticButtons.ElementAt( 0 ).Location.Center.X - 16, staticButtons.ElementAt( 0 ).Location.Center.Y - 16 ) );
            EquipEXP -= Equipment.Equipment.Database.Get( Gear.Head ).ExpCost;

            shirt = new equipment( Equipment.Equipment.Database.Get( Gear.Chest ),
                new Point( staticButtons.ElementAt( 1 ).Location.Center.X - 16, staticButtons.ElementAt( 1 ).Location.Center.Y - 16 ) );
            EquipEXP -= Equipment.Equipment.Database.Get( Gear.Chest ).ExpCost;

            legs = new equipment( Equipment.Equipment.Database.Get( Gear.Legs ),
                new Point( staticButtons.ElementAt( 2 ).Location.Center.X - 16, staticButtons.ElementAt( 2 ).Location.Center.Y - 16 ) );
            EquipEXP -= Equipment.Equipment.Database.Get( Gear.Legs ).ExpCost;



            //TODO: set stats ratios based on class here

            Inventory myInventory = User.Instance.CurrentCharacter.InventoryItems;
            int yPlace = 0;
            int xPlace = 0;

            foreach ( var enumm in EnumUtil.GetValues<WeaponType>() )
            {
                if ( myInventory[ enumm ] > 0 )
                {
                    equips.Add( new equipment( Equipment.Equipment.Database.Get( enumm ), new Point( xPlace * Tile.WIDTH, yPlace * Tile.HEIGHT ) ) );
                    yPlace = Increment( yPlace );
                    if ( yPlace == 0 )
                        xPlace++;
                }
            }
            foreach ( var enumm in EnumUtil.GetValues<HeadType>() )
            {
                if ( myInventory[ enumm ] > 0 )
                {
                    equips.Add( new equipment( Equipment.Equipment.Database.Get( enumm ), new Point( xPlace * Tile.WIDTH, yPlace * Tile.HEIGHT ) ) );
                    yPlace = Increment( yPlace );
                    if ( yPlace == 0 )
                        xPlace++;
                }
            }
            foreach ( var enumm in EnumUtil.GetValues<ChestType>() )
            {
                if ( myInventory[ enumm ] > 0 )
                {
                    equips.Add( new equipment( Equipment.Equipment.Database.Get( enumm ), new Point( xPlace * Tile.WIDTH, yPlace * Tile.HEIGHT ) ) );
                    yPlace = Increment( yPlace );
                    if ( yPlace == 0 )
                        xPlace++;
                }
            }
            foreach ( var enumm in EnumUtil.GetValues<LegsType>() )
            {
                if ( myInventory[ enumm ] > 0 )
                {
                    equips.Add( new equipment( Equipment.Equipment.Database.Get( enumm ), new Point( xPlace * Tile.WIDTH, yPlace * Tile.HEIGHT ) ) );
                    yPlace = Increment( yPlace );
                    if ( yPlace == 0 )
                        xPlace++;
                }
            }
        }

        private int Increment( int i )
        {
            if ( i == 9 )
                return 0;
            else
                return ++i;
        }

        private void LoadContent( ContentManager content )
        {
            monospace = content.Load<SpriteFont>( "Fonts/MonoSpace" );
            background = content.Load<Texture2D>( "Backgrounds/MainMenu" );

            buttons.AddLast( new Button( monospace, "-", Screen.Width * 11 / DENOM, Screen.Height * 4 / DENOM, 25, 25 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( HP_DOWN_CLICK );
            buttons.AddLast( new Button( monospace, "+", Screen.Width * 13 / DENOM - 12, Screen.Height * 4 / DENOM, 25, 25 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( HP_UP_CLICK );
            buttons.AddLast( new Button( monospace, "10", Screen.Width * 13 / DENOM + 13, Screen.Height * 4 / DENOM, 25, 25 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( BIG_HP_UP_CLICK );
            buttons.AddLast( new Button( monospace, "-", Screen.Width * 11 / DENOM, Screen.Height * 6 / DENOM, 25, 25 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( ATK_DOWN_CLICK );
            buttons.AddLast( new Button( monospace, "+", Screen.Width * 13 / DENOM - 12, Screen.Height * 6 / DENOM, 25, 25 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( ATK_UP_CLICK );
            buttons.AddLast( new Button( monospace, "10", Screen.Width * 13 / DENOM + 13, Screen.Height * 6 / DENOM, 25, 25 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( BIG_ATK_UP_CLICK );
            //buttons.AddLast( new Button( monospace, "-", Screen.Width * 11 / DENOM, Screen.Height * 8 / DENOM, 25, 25 ) );
            //buttons.Last.Value.On_Button_Click += new Button.ButtonClick( DEX_DOWN_CLICK );
            //buttons.AddLast( new Button( monospace, "+", Screen.Width * 13 / DENOM - 12, Screen.Height * 8 / DENOM, 25, 25 ) );
            //buttons.Last.Value.On_Button_Click += new Button.ButtonClick( DEX_UP_CLICK );
            //buttons.AddLast( new Button( monospace, "10", Screen.Width * 13 / DENOM + 13, Screen.Height * 8 / DENOM, 25, 25 ) );
            //buttons.Last.Value.On_Button_Click += new Button.ButtonClick( BIG_DEX_UP_CLICK );
            buttons.AddLast( new Button( monospace, "Save and Exit", Screen.Width * 11 / DENOM, Screen.Height * 10 / DENOM, 150, 50 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( SAVE_CLICK );
            buttons.AddLast( new Button( monospace, "Exit", Screen.Width * 11 / DENOM, Screen.Height * 12 / DENOM, 150, 50 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( EXIT_CLICK );

            staticButtons.AddLast( new Button( monospace, "Helm", Screen.Width * 7 / DENOM, Screen.Height * 3 / DENOM, 60, 60 ) );
            staticButtons.AddLast( new Button( monospace, "Top", Screen.Width * 7 / DENOM, Screen.Height * 7 / DENOM, 60, 60 ) );
            staticButtons.AddLast( new Button( monospace, "Legs", Screen.Width * 7 / DENOM, Screen.Height * 11 / DENOM, 60, 60 ) );
            staticButtons.AddLast( new Button( monospace, "Weap", Screen.Width * 9 / DENOM, Screen.Height * 8 / DENOM, 60, 60 ) );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            MouseState ms = Mouse.GetState();

            if ( ms.RightButton == ButtonState.Pressed )
            {
                if ( clicked != null )
                {
                    clicked = null;
                }
                Button target = getButtonClicked( new Point( ms.X, ms.Y ) );
                if ( target != null )
                {
                    switch ( target.Text )
                    {
                        case "Weap":
                            EquipEXP += weap.equip.ExpCost;
                            weap.equip = Equipment.Equipment.Database.Get( WeaponType.None );
                            return;

                        case "Helm":
                            EquipEXP += helm.equip.ExpCost;
                            helm.equip = Equipment.Equipment.Database.Get( HeadType.None );
                            return;

                        case "Top":
                            EquipEXP += shirt.equip.ExpCost;
                            shirt.equip = Equipment.Equipment.Database.Get( ChestType.None );
                            return;

                        case "Legs":
                            EquipEXP += legs.equip.ExpCost;
                            legs.equip = Equipment.Equipment.Database.Get( LegsType.None );
                            return;
                    }
                }
            }

            foreach ( equipment e in equips )
            {
                Point p = new Point( ms.X / Tile.WIDTH, ms.Y / Tile.HEIGHT );
                if ( p.X == e.location.X / Tile.WIDTH && p.Y == e.location.Y / Tile.HEIGHT )
                {
                    selected = e;
                    if ( ms.LeftButton == ButtonState.Pressed && selected.equip.ExpCost < EquipEXP )
                    {
                        clicked = selected.equip;
                    }
                }
            }

            if ( ms.LeftButton == ButtonState.Pressed && clicked != null )
            {
                Button target = getButtonClicked( new Point( ms.X, ms.Y ) );
                if ( target != null )
                {
                    if ( clicked is Weapon && target.Text == "Weap" )
                    {
                        EquipEXP += weap.equip.ExpCost;
                        weap = new equipment( Equipment.Equipment.Database.Get( WeaponType.None ), new Point( 0 * Tile.WIDTH, 0 * Tile.HEIGHT ) );
                        weap.equip = (Weapon)clicked;
                        clicked = null;
                        weap.location = new Point( target.Location.Center.X - 16, target.Location.Center.Y - 16 );
                        EquipEXP -= weap.equip.ExpCost;
                    }
                    else if ( clicked is Helmet && target.Text == "Helm" )
                    {
                        EquipEXP += helm.equip.ExpCost;
                        helm = new equipment( Equipment.Equipment.Database.Get( HeadType.None ), new Point( 0 * Tile.WIDTH, 0 * Tile.HEIGHT ) );
                        helm.equip = (Helmet)clicked;
                        clicked = null;
                        helm.location = new Point( target.Location.Center.X - 16, target.Location.Center.Y - 16 );
                        EquipEXP -= helm.equip.ExpCost;
                    }
                    else if ( clicked is Shirt && target.Text == "Top" )
                    {
                        EquipEXP += shirt.equip.ExpCost;
                        shirt = new equipment( Equipment.Equipment.Database.Get( ChestType.None ), new Point( 0 * Tile.WIDTH, 0 * Tile.HEIGHT ) );
                        shirt.equip = (Shirt)clicked;
                        clicked = null;
                        shirt.location = new Point( target.Location.Center.X - 16, target.Location.Center.Y - 16 );
                        EquipEXP -= shirt.equip.ExpCost;
                    }
                    else if ( clicked is Leggings && target.Text == "Legs" )
                    {
                        EquipEXP += legs.equip.ExpCost;
                        legs = new equipment( Equipment.Equipment.Database.Get( LegsType.None ), new Point( 0 * Tile.WIDTH, 0 * Tile.HEIGHT ) );
                        legs.equip = (Leggings)clicked;
                        clicked = null;
                        legs.location = new Point( target.Location.Center.X - 16, target.Location.Center.Y - 16 );
                        EquipEXP -= legs.equip.ExpCost;
                    }
                }
            }

            foreach ( Button btn in buttons )
                btn.Update();
        }

        public Button getButtonClicked( Point p )
        {
            foreach ( Button btn in staticButtons )
            {
                if ( p.X >= btn.Location.Left && p.X < btn.Location.Right
                    && p.Y >= btn.Location.Top && p.Y < btn.Location.Bottom )
                    return btn;
            }

            return null;
        }

        private Stats addGear()
        {
            return weap.equip.Stats + helm.equip.Stats
                + shirt.equip.Stats + legs.equip.Stats;
        }

        public override void Draw( Canvas canvas )
        {
            MouseState ms = Mouse.GetState();

            canvas.Draw( background, new Rectangle( 0, 0, Screen.Width, Screen.Height ), Color.White );
            canvas.DrawString( monospace, "Health", new Vector2( Screen.Width * 11 / DENOM, Screen.Height * 3.25f / DENOM ), Color.White );
            canvas.DrawString( monospace, "Attack", new Vector2( Screen.Width * 11 / DENOM, Screen.Height * 5.25f / DENOM ), Color.White );
            //canvas.DrawString( monospace, "Dexterity", new Vector2( Screen.Width * 11 / DENOM, Screen.Height * 7.25f / DENOM ), Color.White );
            canvas.DrawString( monospace, "EXP:", new Vector2( Screen.Width * 11 / DENOM, Screen.Height * 2 / DENOM ), Color.White );

            canvas.DrawString( monospace, Stats.MaxHealth.ToString(), new Vector2( Screen.Width * 11 / DENOM + 30, Screen.Height * 4 / DENOM + 4 ), Color.White );
            canvas.DrawString( monospace, Stats.Offense.ToString(), new Vector2( Screen.Width * 11 / DENOM + 30, Screen.Height * 6 / DENOM + 4 ), Color.White );
            //canvas.DrawString( monospace, Stats.Dexterity.ToString(), new Vector2( Screen.Width * 11 / DENOM + 30, Screen.Height * 8 / DENOM + 4 ), Color.White );
            canvas.DrawString( monospace, ( Stats.TotalExp - Stats.SpentExp ).ToString(), new Vector2( Screen.Width * 12 / DENOM, Screen.Height * 2 / DENOM + 4 ), Color.White );

            String p = ( character.BaseStats.MaxHealth + Stats.MaxHealth + addGear().MaxHealth ).ToString();
            canvas.DrawString( monospace, p, new Vector2( Screen.Width * 14 / DENOM, Screen.Height * 4 / DENOM + 4 ), Color.White );
            p = ( character.BaseStats.Offense + Stats.Offense + addGear().Offense ).ToString();
            canvas.DrawString( monospace, p, new Vector2( Screen.Width * 14 / DENOM, Screen.Height * 6 / DENOM + 4 ), Color.White );
            //p = ( character.BaseStats.Dexterity + Stats.Dexterity + addGear().Dexterity ).ToString();
            //canvas.DrawString( monospace, p, new Vector2( Screen.Width * 14 / DENOM, Screen.Height * 8 / DENOM + 4 ), Color.White );

            foreach ( Button btn in buttons )
                btn.Draw( canvas );
            foreach ( Button btn in staticButtons )
                btn.Draw( canvas );

            int offset = 0;
            int column = 0;
            foreach ( equipment wpn in equips )
            {
                canvas.Draw( wpn.equip.Sprite, new Point( column * Tile.WIDTH, offset++ * Tile.HEIGHT ), true );
                if ( offset == 10 )
                {
                    offset = 0;
                    column++;
                }
            }

            if ( clicked != null )
            {
                canvas.Draw( clicked.Sprite, new Point( ms.X - Tile.WIDTH / 2, ms.Y - Tile.HEIGHT / 2 ), true );
            }


            if ( weap != null )
            {
                canvas.Draw( weap.equip.Sprite, weap.location, true );
            }

            if ( helm != null )
            {
                canvas.Draw( helm.equip.Sprite, helm.location, true );
            }

            if ( shirt != null )
            {
                canvas.Draw( shirt.equip.Sprite, shirt.location, true );
            }

            if ( legs != null )
            {
                canvas.Draw( legs.equip.Sprite, legs.location, true );
            }


            if ( selected != null )
            {
                List<String> descriptions = new List<String>();
                int stringLength = WRAP_LENGTH;
                int lineHeight = 0;

                while ( selected.equip.Description.Length > stringLength )
                {
                    descriptions.Add( selected.equip.Description.Substring( stringLength - WRAP_LENGTH, WRAP_LENGTH ) );
                    stringLength += WRAP_LENGTH;
                }
                descriptions.Add( selected.equip.Description.Substring( stringLength - WRAP_LENGTH, selected.equip.Description.Length + WRAP_LENGTH - stringLength ) );


                String name = selected.equip.Name + " - " + selected.equip.ExpCost;

                if ( selected.equip.ExpCost > EquipEXP )
                    canvas.DrawString( monospace, name, new Vector2( 0, ( Screen.Height * 11 / DENOM ) + lineHeight ), Color.Red );
                else
                    canvas.DrawString( monospace, name, new Vector2( 0, ( Screen.Height * 11 / DENOM ) + lineHeight ), Color.White );

                lineHeight += DRAWING_OFFSET;

                foreach ( String str in descriptions )
                {
                    canvas.DrawString( monospace, str, new Vector2( 0, ( Screen.Height * 11 / DENOM ) + lineHeight ), Color.White );
                    lineHeight += DRAWING_OFFSET;
                }

                if ( selected.equip.Damage != 0 )
                {
                    canvas.DrawString( monospace, "Damage: " + selected.equip.Damage.ToString(), new Vector2( 0, ( Screen.Height * 11 / DENOM ) + lineHeight ), Color.White );
                    lineHeight += DRAWING_OFFSET;
                }

                //if ( selected.equip.Dexterity != 0 )
                //{
                //    canvas.DrawString( monospace, "Dexterity: " + selected.equip.Dexterity.ToString(), new Vector2( 0, ( Screen.Height * 11 / DENOM ) + lineHeight ), Color.White );
                //    lineHeight += DRAWING_OFFSET;
                //}

                if ( selected.equip.Defence != 0 )
                {
                    canvas.DrawString( monospace, "Defense: " + selected.equip.Defence.ToString(), new Vector2( 0, ( Screen.Height * 11 / DENOM ) + lineHeight ), Color.White );
                    lineHeight += DRAWING_OFFSET;
                }

                if ( selected.equip.Health != 0 )
                {
                    canvas.DrawString( monospace, "Health: " + selected.equip.Health.ToString(), new Vector2( 0, ( Screen.Height * 11 / DENOM ) + lineHeight ), Color.White );
                    lineHeight += DRAWING_OFFSET;
                }
            }
        }

        private void HP_DOWN_CLICK()
        {
            if ( Stats.SpentExp > 0 )
            {
                Stats.SpentExp--;
                Stats.MaxHealth -= HP_Ratio;
            }
        }
        private void HP_UP_CLICK()
        {
            if ( Stats.SpentExp < Stats.TotalExp )
            {
                Stats.SpentExp++;
                Stats.MaxHealth += HP_Ratio;
            }
        }
        private void BIG_HP_UP_CLICK()
        {
            if ( ( Stats.SpentExp + 10 ) <= Stats.TotalExp )
            {
                Stats.SpentExp += 10;
                Stats.MaxHealth += 10 * HP_Ratio;
            }
        }
        private void ATK_DOWN_CLICK()
        {
            if ( Stats.SpentExp > 0 )
            {
                Stats.SpentExp--;
                Stats.Offense -= ATK_Ratio;
            }
        }
        private void ATK_UP_CLICK()
        {
            if ( Stats.SpentExp < Stats.TotalExp )
            {
                Stats.SpentExp++;
                Stats.Offense += ATK_Ratio;
            }
        }
        private void BIG_ATK_UP_CLICK()
        {
            if ( ( Stats.SpentExp + 10 ) <= Stats.TotalExp )
            {
                Stats.SpentExp += 10;
                Stats.Offense += 10 * HP_Ratio;
            }
        }
        //private void DEX_DOWN_CLICK()
        //{
        //    if ( Stats.SpentExp > 0 )
        //    {
        //        Stats.SpentExp--;
        //        Stats.Dexterity -= DEX_Ratio;
        //    }
        //}
        //private void DEX_UP_CLICK()
        //{
        //    if ( Stats.SpentExp < Stats.TotalExp )
        //    {
        //        Stats.SpentExp++;
        //        Stats.Dexterity += DEX_Ratio;
        //    }
        //}
        //private void BIG_DEX_UP_CLICK()
        //{
        //    if ( ( Stats.SpentExp + 10 ) <= Stats.TotalExp )
        //    {
        //        Stats.SpentExp += 10;
        //        Stats.Dexterity += 10 * HP_Ratio;
        //    }
        //}

        private void SAVE_CLICK()
        {
            // Update this for OO

            Weapon w = (Weapon)weap.equip;
            Gear.Weapon = w.MyType;
            Helmet h = (Helmet)helm.equip;
            Gear.Head = h.MyType;
            Shirt s = (Shirt)shirt.equip;
            Gear.Chest = s.MyType;
            Leggings p = (Leggings)legs.equip;
            Gear.Legs = p.MyType;

            User.Instance.CurrentCharacter.Gear = Gear;

            User.Instance.CurrentCharacter.Stats = Stats;

            try
            {

                User.Instance.CurrentCharacter.Save();

            }
            catch ( WebException e )
            {
                System.Windows.Forms.MessageBox.Show( e.Message, "Unable to connect to server" );
                Game.Exit();
            }

            Game.Back();
        }

        private void EXIT_CLICK()
        {
            Game.Back();
        }
    }
}
