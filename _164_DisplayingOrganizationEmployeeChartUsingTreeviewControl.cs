﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace _164_DisplayingOrganizationEmployeeChartUsingTreeviewControl
{
    /*
     TREEVIEW CONTROL'ünün SHOWCHECKBOXES özelliği ile TREVIEW'daki her nesnenin yanına CHECKBOX eklenir.
     GetSelectedTreeNodes() method parametresine TreeView1.Nodes[0] bunu verekek daha önceden TREVIEW'a eklediğimiz ROOT nesneleri almış oluruz.
     */

    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeViewItems();
            }
        }

        private void GetTreeViewItems()
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("spGetEmployees", con);
            DataSet ds = new DataSet();
            da.Fill(ds);

            ds.Relations.Add("ChildRows", ds.Tables[0].Columns["ID"], ds.Tables[0].Columns["ManagerId"]);

            foreach (DataRow level1DataRow in ds.Tables[0].Rows)
            {
                if (string.IsNullOrEmpty(level1DataRow["ManagerId"].ToString()))
                {
                    TreeNode parentTreeNode = new TreeNode();
                    parentTreeNode.Text = level1DataRow["Name"].ToString();
                    parentTreeNode.Value = level1DataRow["ID"].ToString();
                    GetChildRows(level1DataRow, parentTreeNode);
                    TreeView1.Nodes.Add(parentTreeNode);
                }
            }
        }

        private void GetChildRows(DataRow dataRow, TreeNode treeNode)
        {
            DataRow[] childRows = dataRow.GetChildRows("ChildRows");
            foreach (DataRow row in childRows)
            {
                TreeNode childTreeNode = new TreeNode();
                childTreeNode.Text = row["Name"].ToString();
                childTreeNode.Value = row["ID"].ToString();
                treeNode.ChildNodes.Add(childTreeNode);

                if (row.GetChildRows("ChildRows").Length > 0)
                {
                    GetChildRows(row, childTreeNode);
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ListBox1.Items.Clear();
            GetSelectedTreeNodes(TreeView1.Nodes[0]);
        }

        private void GetSelectedTreeNodes(TreeNode parentTreeNode)
        {
            if (parentTreeNode.Checked)
            {
                ListBox1.Items.Add(parentTreeNode.Text + " - " + parentTreeNode.Value);
            }
            if (parentTreeNode.ChildNodes.Count > 0)
            {
                foreach (TreeNode childTreeNode in parentTreeNode.ChildNodes)
                {
                    GetSelectedTreeNodes(childTreeNode);
                }
            }
        }
    }
}