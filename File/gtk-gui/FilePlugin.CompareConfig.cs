// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace FilePlugin {
    
    
    public partial class CompareConfig {
        
        private Gtk.VBox vbox1;
        
        private Gtk.Table table1;
        
        private Gtk.ComboBoxEntry diff_tool_combo;
        
        private Gtk.Fixed fixed1;
        
        private Gtk.Label label1;
        
        private Gtk.CheckButton run_term_chk;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget FilePlugin.CompareConfig
            Stetic.BinContainer.Attach(this);
            this.Name = "FilePlugin.CompareConfig";
            // Container child FilePlugin.CompareConfig.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            this.vbox1.BorderWidth = ((uint)(8));
            // Container child vbox1.Gtk.Box+BoxChild
            this.table1 = new Gtk.Table(((uint)(2)), ((uint)(2)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(6));
            // Container child table1.Gtk.Table+TableChild
            this.diff_tool_combo = Gtk.ComboBoxEntry.NewText();
            this.diff_tool_combo.Name = "diff_tool_combo";
            this.table1.Add(this.diff_tool_combo);
            Gtk.Table.TableChild w1 = ((Gtk.Table.TableChild)(this.table1[this.diff_tool_combo]));
            w1.LeftAttach = ((uint)(1));
            w1.RightAttach = ((uint)(2));
            w1.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.fixed1 = new Gtk.Fixed();
            this.fixed1.Name = "fixed1";
            this.fixed1.HasWindow = false;
            this.table1.Add(this.fixed1);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.table1[this.fixed1]));
            w2.TopAttach = ((uint)(1));
            w2.BottomAttach = ((uint)(2));
            w2.XOptions = ((Gtk.AttachOptions)(4));
            w2.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.Xalign = 1F;
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("Diff Program:");
            this.table1.Add(this.label1);
            Gtk.Table.TableChild w3 = ((Gtk.Table.TableChild)(this.table1[this.label1]));
            w3.XOptions = ((Gtk.AttachOptions)(4));
            w3.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.run_term_chk = new Gtk.CheckButton();
            this.run_term_chk.CanFocus = true;
            this.run_term_chk.Name = "run_term_chk";
            this.run_term_chk.Label = Mono.Unix.Catalog.GetString("Run in Terminal");
            this.run_term_chk.DrawIndicator = true;
            this.run_term_chk.UseUnderline = true;
            this.table1.Add(this.run_term_chk);
            Gtk.Table.TableChild w4 = ((Gtk.Table.TableChild)(this.table1[this.run_term_chk]));
            w4.TopAttach = ((uint)(1));
            w4.BottomAttach = ((uint)(2));
            w4.LeftAttach = ((uint)(1));
            w4.RightAttach = ((uint)(2));
            w4.XOptions = ((Gtk.AttachOptions)(4));
            w4.YOptions = ((Gtk.AttachOptions)(4));
            this.vbox1.Add(this.table1);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox1[this.table1]));
            w5.Position = 0;
            w5.Expand = false;
            w5.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.run_term_chk.Clicked += new System.EventHandler(this.OnRunTermChkClicked);
            this.diff_tool_combo.Changed += new System.EventHandler(this.OnDiffToolComboChanged);
        }
    }
}
