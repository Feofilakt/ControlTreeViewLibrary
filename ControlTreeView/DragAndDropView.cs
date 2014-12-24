//using System;
//using System.Windows.Forms;
//using System.Drawing;

//namespace ControlTreeView
//{
//    /// <summary>
//    /// Encapsulates the presentation of dragged node.
//    /// </summary>
//    internal class DragAndDropView
//    {

//        class TransparentForm : Form
//        {
//            internal Bitmap Image { get; set; }
//            protected override void OnPaint(PaintEventArgs e)
//            {
//                Point drawPosition=this.PointToClient(Cursor.Position);
//                drawPosition.Offset(5,5);
//                e.Graphics.DrawImage(Image, drawPosition);
//                base.OnPaint(e);
//            }
//        }

//        internal DragAndDropView()
//        {
//            transparentForm = new TransparentForm();
//            transparentForm.Opacity = 0.5F;
//            transparentForm.TransparencyKey = transparentForm.BackColor;
//            transparentForm.FormBorderStyle = FormBorderStyle.None;
//            transparentForm.ControlBox = false;
//            transparentForm.ShowInTaskbar = false;
//            transparentForm.StartPosition = FormStartPosition.Manual;
//            transparentForm.AutoScaleMode = AutoScaleMode.None;
//            nodePicture = new PictureBox();
//            //transparentForm.Controls.Add(nodePicture);
//            transparentForm.TopMost = true;
//        }

//        private TransparentForm transparentForm;
//        private PictureBox nodePicture;
//        internal CTreeNode Node { get; private set; }
//        internal Point CursorPosition { get; private set; }//do not need
//        /// <summary>
//        /// Indicates whether dragged node is shown.
//        /// </summary>
//        internal bool Enabled { get; private set; }

//        /// <summary>
//        /// Start the DragAndDrop operation of specified node at the specified position.
//        /// </summary>
//        /// <param name="node">The node to DragAndDrop.</param>
//        /// <param name="cursorPosition">The position where DragAndDrop was started.</param>
//        internal void BeginDragging(CTreeNode node, Point cursorPosition)
//        {
//            Enabled = true;
//            this.Node = node;
//            this.CursorPosition = cursorPosition;
//            transparentForm.Location = node.ParentCTreeView.PointToScreen(Point.Empty);
//            //transparentForm.ClientSize = node.CTreeView.Size;
//            transparentForm.MinimumSize = transparentForm.MaximumSize = node.ParentCTreeView.ClientSize;

//            Bitmap nodeBitmap = new Bitmap(node.Control.Width, node.Control.Height);
//            node.Control.DrawToBitmap(nodeBitmap, new Rectangle(new Point(0, 0), node.Control.Size));
//            nodePicture.Image = nodeBitmap;
//            transparentForm.Image = nodeBitmap;

//            transparentForm.Enabled = false;
//            //transparentForm.Show();
//            node.ParentCTreeView.Focus(); //это костыль!
//        }

//        /// <summary>
//        /// Stop the DragAndDrop operation.
//        /// </summary>
//        internal void EndDragging()
//        {
//            Enabled = false;
//            transparentForm.Hide();
//        }

//        /// <summary>
//        /// Set the location of dragged node to show.
//        /// </summary>
//        /// <param name="location"></param>
//        internal void SetLocation(Point location)
//        {
//            transparentForm.Invalidate();
//            location.Offset(5, 5);
//            nodePicture.Location = location;
//        }
//    }
//}