using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoftwareEng
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Last Edited Date: 3/28/13
    /// Last Edited By: Ryan Causey
    /// </summary> 
    public partial class MainWindow : Window
    {
        private List<SimpleAlbumData> listOfAlbums;

        //--didn't know what to call it, so I named it the literal spanish translation
        public SoftwareEng.PhotoBomb bombaDeFotos;

        //--stores the albumImageList index of the default image for albums
        private const short defaultAlbumImageListIndex = 0;

        //--used to specify the UID of the "add new album" icon
        private const int addAlbumID = 0;

        //--A more stable storage for the ID of the user album instead
        //-- of relying on a form's selected items collection
        //private int albumChosenbyUser;

        public MainWindow()
        {
            InitializeComponent();

            //for now the gui will determine filepaths(set to same folder as exe) in case it is ever made a user choice
            String libraryPath = System.IO.Path.Combine(Environment.CurrentDirectory, "photo library");
            bombaDeFotos = new PhotoBomb();
            bombaDeFotos.init(guiConstructorCallback, "albumRC1.xml", "photoRC1.xml", libraryPath);

            listOfAlbums = new List<SimpleAlbumData>();

            

            populateAlbumView(true);
        }

        /*
         * Public property to enable WPF to databind to this list of simple album datas
         * Last edited by Ryan Causey
         * Last Edited Date: 3/28/13
         */
        public List<SimpleAlbumData> ListOfAlbumsProperty
        {
            get
            {
                return listOfAlbums;
            }
        }



        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: simply calls back end
        *********************************************************************************************/
        //RM: why is this here???
        private void guiConstructorCallback(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful
        * return type: void
        * purpose: says generic error, but is used for blowing up the backend and restarting 
        *********************************************************************************************/
        private void guiGenericErrorFunction(ErrorReport status)
        {
            if (status.reportID != ErrorReport.SUCCESS)
            {
                if (Directory.Exists("photo library_backup"))
                {
                    Directory.Delete("photo library_backup", true);
                    bombaDeFotos.rebuildBackendOnFilesystem(new generic_callback(guiGenericErrorFunction));
                }
            }
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: bool that determines whether list is cleared or repopulated
        * return type: void
        * purpose: refreshes list of albums
        *********************************************************************************************/
        private void populateAlbumView(bool refreshView)
        {

            /*
            ListViewItem.ListViewSubItem[] itemHolderSubitems;
            ListViewItem itemHolder = null;

            albumListView.Items.Clear();

            //--code from here to the if statement is to regenerate the 
            //--the "add new album icon" as it's not an album
            itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                new ListViewItem.ListViewSubItem(itemHolder, "Add New Album"),
                new ListViewItem.ListViewSubItem(itemHolder, addAlbumID.ToString() )
                };

            itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
            albumListView.Items.Add(itemHolder);

            */



            if (refreshView == true)
            {
                bombaDeFotos.getAllUserAlbumNames(new getAllUserAlbumNames_callback(guiAlbumsRetrieved));
            }


            
        }

        /*********************************************************************************************
        * Author: Alejandro Sosa
        * parameters: an object of ErrorReport which will be used to check if backend was successful,
        *   and a list <type is SimpleAlbumData> containing data to identify all albums requested
        * return type: void
        * purpose: list of albums returned from the backend
        * Last Edited Date: 3/28/13
        * Last Edited By: Ryan Causey
        *********************************************************************************************/
        public void guiAlbumsRetrieved(ErrorReport status, List<SimpleAlbumData> albumsRetrieved)
        {
            if (status.reportID == ErrorReport.SUCCESS)
            {
                listOfAlbums = albumsRetrieved;

                mainWindowAlbumList.listItemsControl.ItemsSource = listOfAlbums;

                mainWindowAlbumList.listItemsControl.DataContext = listOfAlbums;

                

                //mainWindowAlbumList.albumListView.ItemsSource = listOfAlbums;

                

            }
            else
            {
                //show an Error
            }



                /*
                //--stops the drawing of albumListView
                albumListView.BeginUpdate();

                ListViewItem itemHolder = null;
                ListViewItem.ListViewSubItem[] itemHolderSubitems;

                foreach (SimpleAlbumData singleAlbum in albumsRetrieved)
                {
                    try
                    {
                        itemHolderSubitems = new ListViewItem.ListViewSubItem[]{
                            new ListViewItem.ListViewSubItem(itemHolder, singleAlbum.albumName),
                            new ListViewItem.ListViewSubItem(itemHolder, singleAlbum.UID.ToString() )
                            };

                        itemHolder = new ListViewItem(itemHolderSubitems, defaultAlbumImageListIndex);
                        albumListView.Items.Add(itemHolder);
                    }
                    catch (Exception)
                    {
                        showError("Error: Album Missing");
                    }
                }

                //--resumes drawing of albumListView
                albumListView.EndUpdate();
            }
            else if (status.reportID == ErrorReport.FAILURE)
            {
                showError("Error: Album Missing");
            }
            */
        }


        private void mainWindowDock_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = 1;
        }

        private void Grid_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            mainWindowContextMenu.IsOpen = true;
        }

        private void mainWindowContextMenu_LostMouseCapture(object sender, MouseEventArgs e)
        {
            mainWindowContextMenu.IsOpen = false;
        }

        private void dockHitBox_MouseEnter(object sender, MouseEventArgs e)
        {
            mainWindowDock.Height = Double.NaN;
        }
    }
}
