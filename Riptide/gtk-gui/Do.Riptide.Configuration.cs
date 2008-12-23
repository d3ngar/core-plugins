// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Do.Riptide {
    
    
    public partial class Configuration {
        
        private Gtk.VBox vbox1;
        
        private Gtk.CheckButton check_progress_window;
        
        private Gtk.CheckButton check_torrent_alerts;
        
        private Gtk.HBox hbox4;
        
        private Gtk.Label label1;
        
        private Gtk.FileChooserButton file_directory;
        
        private Gtk.HSeparator hseparator2;
        
        private Gtk.HBox hbox6;
        
        private Gtk.Label label3;
        
        private Gtk.SpinButton spin_download_port;
        
        private Gtk.HBox hbox7;
        
        private Gtk.CheckButton check_max_download;
        
        private Gtk.SpinButton spin_download_speed;
        
        private Gtk.HBox hbox5;
        
        private Gtk.CheckButton check_max_upload;
        
        private Gtk.SpinButton spin_upload_speed;
        
        private Gtk.HSeparator hseparator1;
        
        private Gtk.CheckButton check_force_encryption;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.Riptide.Configuration
            Stetic.BinContainer.Attach(this);
            this.Name = "Do.Riptide.Configuration";
            // Container child Do.Riptide.Configuration.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            this.check_progress_window = new Gtk.CheckButton();
            this.check_progress_window.CanFocus = true;
            this.check_progress_window.Name = "check_progress_window";
            this.check_progress_window.Label = Mono.Unix.Catalog.GetString("Show Progress Window");
            this.check_progress_window.DrawIndicator = true;
            this.check_progress_window.UseUnderline = true;
            this.vbox1.Add(this.check_progress_window);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.vbox1[this.check_progress_window]));
            w1.Position = 0;
            w1.Expand = false;
            w1.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.check_torrent_alerts = new Gtk.CheckButton();
            this.check_torrent_alerts.CanFocus = true;
            this.check_torrent_alerts.Name = "check_torrent_alerts";
            this.check_torrent_alerts.Label = Mono.Unix.Catalog.GetString("Display Torrent Alerts");
            this.check_torrent_alerts.DrawIndicator = true;
            this.check_torrent_alerts.UseUnderline = true;
            this.vbox1.Add(this.check_torrent_alerts);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox1[this.check_torrent_alerts]));
            w2.Position = 1;
            w2.Expand = false;
            w2.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox4 = new Gtk.HBox();
            this.hbox4.Name = "hbox4";
            this.hbox4.Spacing = 6;
            // Container child hbox4.Gtk.Box+BoxChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.Xpad = 15;
            this.label1.Xalign = 0F;
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("Download Directory:");
            this.hbox4.Add(this.label1);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hbox4[this.label1]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child hbox4.Gtk.Box+BoxChild
            this.file_directory = new Gtk.FileChooserButton(Mono.Unix.Catalog.GetString("Select A Directory"), ((Gtk.FileChooserAction)(2)));
            this.file_directory.Name = "file_directory";
            this.hbox4.Add(this.file_directory);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox4[this.file_directory]));
            w4.Position = 1;
            this.vbox1.Add(this.hbox4);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox4]));
            w5.Position = 2;
            w5.Expand = false;
            w5.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hseparator2 = new Gtk.HSeparator();
            this.hseparator2.Name = "hseparator2";
            this.vbox1.Add(this.hseparator2);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.vbox1[this.hseparator2]));
            w6.Position = 3;
            w6.Expand = false;
            w6.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox6 = new Gtk.HBox();
            this.hbox6.Name = "hbox6";
            this.hbox6.Spacing = 6;
            // Container child hbox6.Gtk.Box+BoxChild
            this.label3 = new Gtk.Label();
            this.label3.Name = "label3";
            this.label3.Xpad = 15;
            this.label3.Xalign = 0F;
            this.label3.LabelProp = Mono.Unix.Catalog.GetString("Download Port:");
            this.hbox6.Add(this.label3);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.hbox6[this.label3]));
            w7.Position = 0;
            // Container child hbox6.Gtk.Box+BoxChild
            this.spin_download_port = new Gtk.SpinButton(0, 65000, 1);
            this.spin_download_port.CanFocus = true;
            this.spin_download_port.Name = "spin_download_port";
            this.spin_download_port.Adjustment.PageIncrement = 10;
            this.spin_download_port.ClimbRate = 1;
            this.spin_download_port.Numeric = true;
            this.spin_download_port.Value = 3500;
            this.hbox6.Add(this.spin_download_port);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.hbox6[this.spin_download_port]));
            w8.Position = 1;
            w8.Expand = false;
            w8.Fill = false;
            this.vbox1.Add(this.hbox6);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox6]));
            w9.Position = 4;
            w9.Expand = false;
            w9.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox7 = new Gtk.HBox();
            this.hbox7.Name = "hbox7";
            this.hbox7.Spacing = 6;
            // Container child hbox7.Gtk.Box+BoxChild
            this.check_max_download = new Gtk.CheckButton();
            this.check_max_download.CanFocus = true;
            this.check_max_download.Name = "check_max_download";
            this.check_max_download.Label = Mono.Unix.Catalog.GetString("Max Download Speed: (KB/s)");
            this.check_max_download.DrawIndicator = true;
            this.check_max_download.UseUnderline = true;
            this.hbox7.Add(this.check_max_download);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.hbox7[this.check_max_download]));
            w10.Position = 0;
            // Container child hbox7.Gtk.Box+BoxChild
            this.spin_download_speed = new Gtk.SpinButton(0, 900, 1);
            this.spin_download_speed.CanFocus = true;
            this.spin_download_speed.Name = "spin_download_speed";
            this.spin_download_speed.Adjustment.PageIncrement = 10;
            this.spin_download_speed.ClimbRate = 1;
            this.spin_download_speed.Numeric = true;
            this.hbox7.Add(this.spin_download_speed);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.hbox7[this.spin_download_speed]));
            w11.Position = 1;
            w11.Expand = false;
            w11.Fill = false;
            this.vbox1.Add(this.hbox7);
            Gtk.Box.BoxChild w12 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox7]));
            w12.Position = 5;
            w12.Expand = false;
            w12.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox5 = new Gtk.HBox();
            this.hbox5.Name = "hbox5";
            this.hbox5.Spacing = 6;
            // Container child hbox5.Gtk.Box+BoxChild
            this.check_max_upload = new Gtk.CheckButton();
            this.check_max_upload.CanFocus = true;
            this.check_max_upload.Name = "check_max_upload";
            this.check_max_upload.Label = Mono.Unix.Catalog.GetString("Max Upload Speed: (KB/s)");
            this.check_max_upload.DrawIndicator = true;
            this.check_max_upload.UseUnderline = true;
            this.hbox5.Add(this.check_max_upload);
            Gtk.Box.BoxChild w13 = ((Gtk.Box.BoxChild)(this.hbox5[this.check_max_upload]));
            w13.Position = 0;
            // Container child hbox5.Gtk.Box+BoxChild
            this.spin_upload_speed = new Gtk.SpinButton(0, 900, 1);
            this.spin_upload_speed.CanFocus = true;
            this.spin_upload_speed.Name = "spin_upload_speed";
            this.spin_upload_speed.Adjustment.PageIncrement = 10;
            this.spin_upload_speed.ClimbRate = 1;
            this.spin_upload_speed.Numeric = true;
            this.hbox5.Add(this.spin_upload_speed);
            Gtk.Box.BoxChild w14 = ((Gtk.Box.BoxChild)(this.hbox5[this.spin_upload_speed]));
            w14.Position = 1;
            w14.Expand = false;
            w14.Fill = false;
            this.vbox1.Add(this.hbox5);
            Gtk.Box.BoxChild w15 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox5]));
            w15.Position = 6;
            w15.Expand = false;
            w15.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hseparator1 = new Gtk.HSeparator();
            this.hseparator1.Name = "hseparator1";
            this.vbox1.Add(this.hseparator1);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.vbox1[this.hseparator1]));
            w16.Position = 7;
            w16.Expand = false;
            w16.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.check_force_encryption = new Gtk.CheckButton();
            this.check_force_encryption.CanFocus = true;
            this.check_force_encryption.Name = "check_force_encryption";
            this.check_force_encryption.Label = Mono.Unix.Catalog.GetString("Force Encryption");
            this.check_force_encryption.DrawIndicator = true;
            this.check_force_encryption.UseUnderline = true;
            this.vbox1.Add(this.check_force_encryption);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.vbox1[this.check_force_encryption]));
            w17.Position = 8;
            w17.Expand = false;
            w17.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.check_progress_window.Clicked += new System.EventHandler(this.OnCheckProgressWindowClicked);
            this.check_torrent_alerts.Clicked += new System.EventHandler(this.OnCheckTorrentAlertsClicked);
            this.file_directory.SelectionChanged += new System.EventHandler(this.on_download_directory_changed);
            this.spin_download_port.ValueChanged += new System.EventHandler(this.OnSpinDownloadPortValueChanged);
            this.check_max_download.Clicked += new System.EventHandler(this.OnCheckMaxDownloadClicked);
            this.spin_download_speed.ValueChanged += new System.EventHandler(this.OnSpinDownloadSpeedValueChanged);
            this.check_max_upload.Clicked += new System.EventHandler(this.OnCheckMaxUploadClicked);
            this.spin_upload_speed.ValueChanged += new System.EventHandler(this.OnSpinUploadSpeedValueChanged);
            this.check_force_encryption.Clicked += new System.EventHandler(this.OnCheckForceEncryptionClicked);
        }
    }
}
