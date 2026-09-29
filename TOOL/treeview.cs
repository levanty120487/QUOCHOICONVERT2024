using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1
{
    public partial class treeview  : RJForms.RJChildForm
    {
        public treeview()
        {
            InitializeComponent();
            InitializeTreeView();
        }
        private void InitializeTreeView()
        {
            TreeView treeView = new TreeView();
          //  treeView1.Dock = DockStyle.Left;

            // Tạo danh sách các danh mục
            List<CategoryTree> categories = new List<CategoryTree>
            {
                new CategoryTree { Id = 1, Name = "Thư mục A", ParentId = null },
                new CategoryTree { Id = 2, Name = "Thư mục B", ParentId = null },
                new CategoryTree { Id = 3, Name = "Thư mục A1", ParentId = 1 },
                new CategoryTree { Id = 4, Name = "Thư mục A2", ParentId = 1 },
                new CategoryTree { Id = 5, Name = "Thư mục B1", ParentId = 2 },
                new CategoryTree { Id = 6, Name = "Thư mục A1.1", ParentId = 3 }
            };

            // Lấy các node gốc (ParentId = null)
            var rootCategories = categories.Where(c => c.ParentId == null).ToList();

            // Duyệt qua danh sách root để thêm vào TreeView
            foreach (var rootCategory in rootCategories)
            {
                TreeNode rootNode = new TreeNode(rootCategory.Name);
                rootNode.Tag = rootCategory.Id; // Đính kèm ID của Category để dễ dàng thao tác sau này

                // Gọi hàm đệ quy để thêm các node con
                AddChildNodes(rootNode, categories);

                treeView.Nodes.Add(rootNode);
            }

            treeView.ExpandAll();
            this.Controls.Add(treeView);
        }

        // Hàm đệ quy để thêm các node con
        private void AddChildNodes(TreeNode parentNode, List<CategoryTree> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.Name);
                childNode.Tag = childCategory.Id;

                parentNode.Nodes.Add(childNode);

                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodes(childNode, categories);
            }
        }
    }
    // Lớp Category để đại diện cho một danh mục
    public class CategoryTree
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

}



