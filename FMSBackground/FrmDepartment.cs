﻿using FileSystem.BLL;
using FileSystem.Entity;
using FMSBackground.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace FMSBackground
{
    public partial class FrmDepartment : BaseForm
    {
        TreeNode _selectedNode = null;//保存最后一次点击的节点
        DepartmentLogic _depLogic = new DepartmentLogic();
        bool _bAdd = false;


        public FrmDepartment()
        {
            InitializeComponent();
        }


        private void FrmDepartment_Load(object sender, EventArgs e)
        {
            InitDepartmentTree();//初始化部门树
        }


        private void InitDepartmentTree()
        {
            //创建根节点
            TreeNode root = tvDep.Nodes.Add("部门管理");
            AddChildNode(root);
        }

        private void AddChildNode (TreeNode pNode)
        {
            List<Department> list = _depLogic.GetDepartments();
            foreach (var d in list)
            {
                TreeNode node = new TreeNode(d.DepartmentName);
                node.Tag = d;
                node.Name = d.DepartmentID.ToString();
                pNode.Nodes.Add(node);
                foreach (var p in d.Positions)
                {
                    TreeNode child = new TreeNode(p.PositionName);
                    child.Tag = p;
                    child.Name = p.PositionID.ToString();
                    node.Nodes.Add(child);
                }
            }
            tvDep.ExpandAll();
        }



        private void tvDep_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Department d = e.Node.Tag as Department;
            if (d == null)
            {
                //gang wei
                d = e.Node.Parent.Tag as Department;
                if (d == null) return;
                Position p = e.Node.Tag as Position;
                Console.WriteLine(d.DepartmentName + "-----" + p.PositionName);
            }
           else
            {
                //bu men
                _selectedNode = e.Node;
                txtDepName.Text = d.DepartmentName;
            }

            //Department pd = _depLogic.GetParentDepartmentByPID((int)d.DepartmentPID);
            //if (pd == null) return;
            //cboDepartment.Text = pd.DepartmentName;
        }
        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, AuthEventArgs e)
        {
            if (!e.OK) return;
            Debug.WriteLine("btnAdd_Click");
            if (!e.OK) return;
            _bAdd = true;
            txtDepName.Text = string.Empty;
            gbDetail.Enabled = true;//添加时解锁界面/启用界面
            pnlAction.Enabled = false;//禁用动作面板
        }
        private bool AddDepartment()
        {
            Department d = new Department
            {
                DepartmentName = txtDepName.Text,
            };
            bool ok = _depLogic.AddDepartment(d);
            if (ok)
            {
                ReloadTree();
            }
            return true;
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_bAdd)
            {
                if (!AddDepartment()) return;
                _bAdd = false;
            }
            else
            {
                UpdateDepartment();
            }
            gbDetail.Enabled = false;//保存完了加锁界面/禁用界面
            pnlAction.Enabled = true;//动作面板要启用
        }

        private void UpdateDepartment()
        {
            if (_selectedNode == null) return;
            Department d = _selectedNode.Tag as Department;
            if (d == null) return;
            d.DepartmentName = txtDepName.Text;
            if (_depLogic.EditDepartment(d))
            {
                ReloadTree();
            }

        }

        private void btnDelete_Click(object sender, AuthEventArgs e)
        {
            if (e.OK)
            {
                //you quan xian
            }
            if (_selectedNode == null) return;
            Department d = _selectedNode.Tag as Department;
            bool ok = _depLogic.DeleteDepartmentByID(d.DepartmentID);
            if (ok)
            {
                tvDep.Nodes.Remove(_selectedNode);
            }
        }
        private void ReloadTree()
        {
            tvDep.Nodes.Clear();//清空所有节点
            InitDepartmentTree();//初始化所有节点
            tvDep.ExpandAll();//展开所有 节点
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            gbDetail.Enabled = false;//取消时详情面板要禁用
            pnlAction.Enabled = true;//启用动作面板
        }

        private void btnEdit_Click(object sender, AuthEventArgs e)
        {
            gbDetail.Enabled = true;//编辑时解锁界面/启用界面
            pnlAction.Enabled = false;
        }

        private void btnEditUser_Click(object sender, EventArgs e)
        {
            IncludeUser include = new IncludeUser();
            include.Show();
        }
    }
}