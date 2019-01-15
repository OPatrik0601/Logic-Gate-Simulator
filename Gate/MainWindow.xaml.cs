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
using System.Timers;
using System.Windows.Threading;

namespace Gate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int TICK_TIME = 10000000/100; // 1/100 sec
        private int add_selectedtype = 0;
        /*
         * 0 - Nothing
         * 1 - Switch
         * 2 - Bulb
         * 3 - AND
         * 4 - OR
         * 5 - Inverter
         * 6 - XOR
        */
        private DispatcherTimer timer;
        private List<Mod> ModList = new List<Mod>();

        private List<Connection> conns = new List<Connection>();
        private Connection currentConn = null; //Current connect selection if there is any

        private Mod PreviewMod = null;
        private Mod SelectedMod = null;

        public MainWindow()
        {
            InitializeComponent();
            AddItems();
            ChangeInfo("");
        }

        private void timer_Tick(object sender, object e)
        {
            //Tick
            content.Children.Clear();

            ShowSelect();

            //Draw controls
            for (int i=0;i<ModList.Count;i++)
            {
                showMod(ModList[i]);

                //Refresh bulb images
                if(typeof(Bulb) == ModList[i].GetType())
                {
                    ModList[i].isActive(conns.ToArray());
                }
            }

            //Draw preview
            if (PreviewMod != null && isValidPosition(PreviewMod.x, PreviewMod.y))
            {
                showMod(PreviewMod, false);
            }

            //Draw connections
            for (int i = 0; i < conns.Count; i++)
            {
                Line l = new Line();
                if(conns[i].output.isActive(conns.ToArray()))
                    l.Stroke = Brushes.Green;
                else
                    l.Stroke = Brushes.Red;

                l.X1 = conns[i].input.x - 30;
                l.X2 = conns[i].output.x + 30;
                l.Y1 = conns[i].input.y;
                l.Y2 = conns[i].output.y;

                l.StrokeThickness = 2;
                content.Children.Add(l);
            }

            //Refresh info
            if (ModList.Count == 0 && add_selectedtype == 0)
                ChangeInfo("You can add controls by selecting it at the top left corner and pressing 'Select'.");
            else if (add_selectedtype > 0)
            {
                string[] name = GetType(add_selectedtype).ToString().Split('.');
                ChangeInfo("You have selected a(n) " + name[1] + ". Put it to the gray area, or cancel it by pressing the right mouse button.");
            }
            else if (ModList.Count >= 2 && conns.Count == 0 && currentConn == null)
            {
                ChangeInfo("You can connect two controls if you press the right mouse button on them.");
            }
            else if (currentConn != null)
                ChangeInfo("Press the right mouse button to connect the currently selected control's output to an another's input.");
            else if(SelectedMod != null && SelectedMod.GetType() == typeof(Switch))
                ChangeInfo("You can change the switch's value by pressing the left mouse button on it.");
            else
                ChangeInfo("");

            //Cancel button text
            if (currentConn != null)
            {
                remove_conn.Content = "Cancel connection";
                remove_conn.IsEnabled = true;
            }
            else if (add_selectedtype != 0)
            {
                remove_conn.Content = "Cancel add control";
                remove_conn.IsEnabled = true;
            }
            else
            {
                remove_conn.IsEnabled = false;
            }


            timer.Start();
        }

        private bool isValidPosition(int x, int y)
        {
            //The x,y is in the Canvas?
            if (x > 0 + 30 &&
                y > 0 + 30 &&
                x < content.ActualWidth - 30 &&
                y < content.ActualHeight - 30)
                return true;
            else return false;
        }

        private void showMod(Mod mod, bool clickable)
        {
            //Show the given mod
            Image img = new Image();
            img.Width = 60;
            img.Height = 60;
            img.Source = getImage(mod.imagePath);
            content.Children.Add(img);
            ChangeLocation(img, mod.x-30, mod.y-30);
            if (clickable)
            {
                if (typeof(Switch) == mod.GetType())
                {
                    Switch convert = (Switch)mod;
                    img.MouseLeftButtonDown += convert.OnClick;
                }

                img.MouseRightButtonDown += (sender, e) => onRightClick(sender, e, mod);
                img.MouseLeftButtonDown += (sender, e) => Select(sender, e, mod);
            }
            img.IsHitTestVisible = true;
        }

        private void Select(object sender, MouseButtonEventArgs e, Mod mod)
        {
            SelectedMod = mod;
        }

        private void ShowSelect()
        {
            //Show the selection circle (Red if it's selected, brown-ish if it's selected for connection
            if (SelectedMod != null)
            {
                Image img = new Image();
                img.Width = 80;
                img.Height = 80;
                if (currentConn == null)
                    img.Source = getImage("selection.png");
                else
                    img.Source = getImage("selection_connect.png");

                content.Children.Add(img);
                ChangeLocation(img, SelectedMod.x - 40, SelectedMod.y - 40);
            }
        }

        private void onRightClick(object sender, MouseButtonEventArgs e, Mod mod)
        {
            if (currentConn == null)
            {
                if (mod.IsThereOutput)
                {
                    currentConn = new Connection();
                    currentConn.output = mod;
                    Console.WriteLine("Selected control 1");
                } else
                    MessageBox.Show("This control does not have any output!", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (!IsConnectionExsist(mod, currentConn.output))
                {
                    if (currentConn.output != mod)
                    {
                        if (!mod.IsInputFull(conns.ToArray())) //if the clicked mod has any available inputs use it as an input
                        {
                            currentConn.input = mod;
                            conns.Add(currentConn);
                            currentConn = null;
                            Console.WriteLine("Connection added");
                        }
                        else
                        {
                            RemoveAllSelection();
                            MessageBox.Show("This control can not handle more inputs!", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    } else
                    {
                        RemoveAllSelection();
                    }
                } else
                {
                    RemoveAllSelection();
                    MessageBox.Show("This connection is already exists!", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            SelectedMod = mod;
        }

        private bool IsConnectionExsist(Mod m1, Mod m2)
        {
            for(int i=0;i<conns.Count;i++)
            {
                if (conns[i].input == m1 && conns[i].output == m2 ||
                    conns[i].input == m2 && conns[i].output == m1)
                    return true;
            }

            return false;
        }

        private void showMod(Mod mod)
        {
            showMod(mod, true);
        }

        private void ChangeInfo(string newInfo)
        {
            Info.Content = newInfo;
        }

        private void AddItems()
        {
            //Add elements to list
            addnew.Items.Add("Switch");
            addnew.Items.Add("Light bulb");
            addnew.Items.Add("AND");
            addnew.Items.Add("OR");
            addnew.Items.Add("Inverter");
            addnew.Items.Add("XOR");
            addnew.SelectedIndex = 0;

            //Set timer
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(TICK_TIME);
            timer.Start();
            timer.Tick += timer_Tick;
        }

        private ImageSource getImage(string FileName)
        {
            //Get image from the app img directory
            try
            {
                return new BitmapImage(new Uri("pack://application:,,,/img/" + FileName));
            } catch(Exception e) {
                Console.WriteLine("ERROR: " + e.StackTrace);
                return null;
            }
        }

        private void ChangeLocation(UIElement element, int x, int y)
        {
            //Change the location of any type of UIElement by x,y coordinate
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }

        private Mod GetType(int ID)
        {
            switch (add_selectedtype)
            {
                case 1:
                    return new Switch();
                case 2:
                    return new Bulb();
                case 3:
                    return new And();
                case 4:
                    return new Or();
                case 5:
                    return new Inverter();
                case 6:
                    return new Xor();
            }

            return null;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            //Selected type ID
            add_selectedtype = addnew.SelectedIndex + 1;
            PreviewMod = GetType(add_selectedtype);
            RemoveAllSelection();
        }

        private void remove_selected_conn_Click(object sender, RoutedEventArgs e)
        {
            /*Go trough all and connections and remove where we can find the selected
            Basically "Remove the control" but without the mod removing part */

            for (int i = conns.Count - 1; i >= 0; i--)
            {
                if (conns[i].input == SelectedMod || conns[i].output == SelectedMod)
                {
                    conns.Remove(conns[i]);
                }
            }
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            add_selectedtype = 0;
            currentConn = null;
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            ModList.Clear();
            conns.Clear();
            content.Children.Clear();
            RemoveAllSelection();
        }

        private void RemoveAllSelection()
        {
            SelectedMod = null;
            currentConn = null;
        }

        private void addMod(object sender, MouseEventArgs e)
        {
            //Put selected type on canvas
            if (add_selectedtype != 0)
            {
                try
                {
                    Mod temp = GetType(add_selectedtype);
                    temp.setCords((int)e.GetPosition(content).X, (int)e.GetPosition(content).Y);
                    //in canvas?
                    if (isValidPosition(temp.x, temp.y))
                    {
                        ModList.Add(temp);
                        showMod(temp, false);
                        SelectedMod = temp;
                    }
                    add_selectedtype = 0;
                    PreviewMod = null;
                } catch(Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void content_MouseMove(object sender, MouseEventArgs e)
        {
            if(add_selectedtype != 0 && PreviewMod != null)
            {
                PreviewMod.setCords((int)e.GetPosition(content).X, (int)e.GetPosition(content).Y);
            }
        }

        private void remove_selected_Click(object sender, RoutedEventArgs e)
        {
            //Go trough all mods and connections and remove the selected one

            for(int i=conns.Count-1;i>=0;i--)
            {
                if(conns[i].input == SelectedMod || conns[i].output == SelectedMod)
                {
                    conns.Remove(conns[i]);
                }
            }

            ModList.Remove(SelectedMod);
            SelectedMod = null;
        }

        private void content_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PreviewMod != null)
            {
                PreviewMod = null;
                add_selectedtype = 0;
            }
        }
    }
}
