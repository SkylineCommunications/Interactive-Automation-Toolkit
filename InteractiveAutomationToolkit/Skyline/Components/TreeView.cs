﻿namespace Skyline.DataMiner.DeveloperCommunityLibrary.InteractiveAutomationToolkit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Automation;

	using Net.AutomationUI.Objects;

	/// <summary>
	///  A tree view structure.
	/// </summary>
	public class TreeView : InteractiveWidget, ITreeView
	{
		private Dictionary<string, bool> checkedItemCache;
		private Dictionary<string, bool> collapsedItemCache; // TODO: should only contain Items with LazyLoading set to true
		private Dictionary<string, TreeViewItem> lookupTable;

		private bool itemsChanged;
		private List<TreeViewItem> changedItems = new List<TreeViewItem>();

		private bool itemsChecked;
		private List<TreeViewItem> checkedItems = new List<TreeViewItem>();

		private bool itemsUnchecked;
		private List<TreeViewItem> uncheckedItems = new List<TreeViewItem>();

		private bool itemsExpanded;
		private List<TreeViewItem> expandedItems = new List<TreeViewItem>();

		private bool itemsCollapsed;
		private List<TreeViewItem> collapsedItems = new List<TreeViewItem>();

		/// <summary>
		///		Initializes a new instance of the <see cref="TreeView" /> class.
		/// </summary>
		/// <param name="treeViewItems"></param>
		public TreeView(IEnumerable<TreeViewItem> treeViewItems)
		{
			Type = UIBlockType.TreeView;
			Items = treeViewItems;
		}

		/// <inheritdoc />
		public event EventHandler<ChangedEventArgs> Changed
		{
			add
			{
				OnChanged += value;
				WantsOnChange = true;
			}

			remove
			{
				OnChanged -= value;
				if (OnChanged == null || !OnChanged.GetInvocationList().Any())
				{
					WantsOnChange = false;
				}
			}
		}

		private event EventHandler<ChangedEventArgs> OnChanged;

		/// <inheritdoc />
		public event EventHandler<CheckedEventArgs> Checked
		{
			add
			{
				OnChecked += value;
				WantsOnChange = true;
			}

			remove
			{
				OnChecked -= value;
				if (OnChecked == null || !OnChecked.GetInvocationList().Any())
				{
					WantsOnChange = false;
				}
			}
		}

		private event EventHandler<CheckedEventArgs> OnChecked;

		/// <inheritdoc />
		public event EventHandler<UncheckedEventArgs> Unchecked
		{
			add
			{
				OnUnchecked += value;
				WantsOnChange = true;
			}

			remove
			{
				OnUnchecked -= value;
				if (OnUnchecked == null || !OnUnchecked.GetInvocationList().Any())
				{
					WantsOnChange = false;
				}
			}
		}

		private event EventHandler<UncheckedEventArgs> OnUnchecked;

		/// <inheritdoc />
		public event EventHandler<ExpandedEventArgs> Expanded
		{
			add
			{
				OnExpanded += value;
			}

			remove
			{
				OnExpanded -= value;
			}
		}

		private event EventHandler<ExpandedEventArgs> OnExpanded;

		/// <inheritdoc />
		public event EventHandler<CollapsedEventArgs> Collapsed
		{
			add
			{
				OnCollapsed += value;
			}

			remove
			{
				OnCollapsed -= value;
			}
		}

		private event EventHandler<CollapsedEventArgs> OnCollapsed;

		/// <inheritdoc />
		public void Collapse()
		{
			foreach (var item in GetAllItems())
			{
				item.IsCollapsed = true;
			}
		}

		/// <inheritdoc />
		public void Expand()
		{
			foreach (var item in GetAllItems())
			{
				item.IsCollapsed = false;
			}
		}

		/// <inheritdoc />
		public IEnumerable<TreeViewItem> Items
		{
			get
			{
				return BlockDefinition.TreeViewItems;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				BlockDefinition.TreeViewItems = new List<TreeViewItem>(value);
				UpdateItemCache();
			}
		}

		/// <inheritdoc />
		public IEnumerable<TreeViewItem> CheckedItems
		{
			get
			{
				return GetCheckedItems();
			}
		}

		/// <inheritdoc />
		public IEnumerable<TreeViewItem> CheckedLeaves
		{
			get
			{
				return GetCheckedItems().Where(x => !x.ChildItems.Any());
			}
		}

		/// <inheritdoc />
		public IEnumerable<TreeViewItem> CheckedNodes
		{
			get
			{
				return GetCheckedItems().Where(x => x.ChildItems.Any());
			}
		}

		/// <inheritdoc />
		public bool TryFindTreeViewItem(string key, out TreeViewItem item)
		{
			item = GetAllItems().FirstOrDefault(x => x.KeyValue.Equals(key));
			return item != null;
		}

		/// <summary>
		/// This method is used to update the cached TreeViewItems and lookup table.
		/// </summary>
		public void UpdateItemCache()
		{
			checkedItemCache = new Dictionary<string, bool>();
			collapsedItemCache = new Dictionary<string, bool>();
			lookupTable = new Dictionary<string, TreeViewItem>();

			foreach (var item in GetAllItems())
			{
				try
				{
					checkedItemCache.Add(item.KeyValue, item.IsChecked);
					if (item.SupportsLazyLoading)
					{
						collapsedItemCache.Add(item.KeyValue, item.IsCollapsed);
					}

					lookupTable.Add(item.KeyValue, item);
				}
				catch (Exception e)
				{
					throw new TreeViewDuplicateItemsException(item.KeyValue, e);
				}
			}
		}

		/// <summary>
		/// Returns all items in the TreeView that are checked.
		/// </summary>
		/// <returns>All checked TreeViewItems in the TreeView.</returns>
		private IEnumerable<TreeViewItem> GetCheckedItems()
		{
			return lookupTable.Values.Where(x => x.ItemType == TreeViewItem.TreeViewItemType.CheckBox && x.IsChecked);
		}

		/// <inheritdoc />
		public IEnumerable<TreeViewItem> GetAllItems()
		{
			List<TreeViewItem> allItems = new List<TreeViewItem>();
			foreach (var item in Items)
			{
				allItems.Add(item);
				allItems.AddRange(GetAllItems(item.ChildItems));
			}

			return allItems;
		}

		/// <summary>
		/// This method is used to recursively go through all the items in the TreeView.
		/// </summary>
		/// <param name="children">List of TreeViewItems to be visited.</param>
		/// <returns>Flat collection containing every item in the provided children collection and all underlying items.</returns>
		private IEnumerable<TreeViewItem> GetAllItems(IEnumerable<TreeViewItem> children)
		{
			List<TreeViewItem> allItems = new List<TreeViewItem>();
			foreach (var item in children)
			{
				allItems.Add(item);
				allItems.AddRange(GetAllItems(item.ChildItems));
			}

			return allItems;
		}

		/// <inheritdoc />
		public IEnumerable<TreeViewItem> GetItems(int depth)
		{
			return GetItems(Items, depth, 0);
		}

		/// <summary>
		/// Returns all TreeViewItems in the TreeView that are located on the provided depth.
		/// </summary>
		/// <param name="children">Items to be checked.</param>
		/// <param name="requestedDepth">Depth that was requested.</param>
		/// <param name="currentDepth">Current depth in the tree.</param>
		/// <returns>All TreeViewItems in the TreeView that are located on the provided depth.</returns>
		private IEnumerable<TreeViewItem> GetItems(IEnumerable<TreeViewItem> children, int requestedDepth, int currentDepth)
		{
			List<TreeViewItem> requestedItems = new List<TreeViewItem>();
			bool depthReached = requestedDepth == currentDepth;
			foreach (TreeViewItem item in children)
			{
				if (depthReached)
				{
					requestedItems.Add(item);
				}
				else
				{
					int newDepth = currentDepth + 1;
					requestedItems.AddRange(GetItems(item.ChildItems, requestedDepth, newDepth));
				}
			}

			return requestedItems;
		}

		/// <inheritdoc />
		public string Tooltip
		{
			get
			{
				return BlockDefinition.TooltipText;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				BlockDefinition.TooltipText = value;
			}
		}

		/// <inheritdoc />
		protected internal override void LoadResult(UIResults uiResults)
		{
			var checkedItemKeys = uiResults.GetCheckedItemKeys(this); // this includes all checked items
			var expandedItemKeys = uiResults.GetExpandedItemKeys(this); // this includes all expanded items with LazyLoading set to true

			// Check for changes
			// Expanded Items
			List<string> newlyExpandedItems = collapsedItemCache.Where(x => expandedItemKeys.Contains(x.Key) && x.Value).Select(x => x.Key).ToList();
			if (newlyExpandedItems.Any() && OnExpanded != null)
			{
				itemsExpanded = true;
				expandedItems = new List<TreeViewItem>();

				foreach (string newlyExpandedItemKey in newlyExpandedItems)
				{
					expandedItems.Add(lookupTable[newlyExpandedItemKey]);
				}
			}

			// Collapsed Items
			List<string> newlyCollapsedItems = collapsedItemCache.Where(x => !expandedItemKeys.Contains(x.Key) && !x.Value).Select(x => x.Key).ToList();
			if (newlyCollapsedItems.Any() && OnCollapsed != null)
			{
				itemsCollapsed = true;
				collapsedItems = new List<TreeViewItem>();

				foreach (string newyCollapsedItemKey in newlyCollapsedItems)
				{
					collapsedItems.Add(lookupTable[newyCollapsedItemKey]);
				}
			}

			// Checked Items
			List<string> newlyCheckedItemKeys = checkedItemCache.Where(x => checkedItemKeys.Contains(x.Key) && !x.Value).Select(x => x.Key).ToList();
			if (newlyCheckedItemKeys.Any() && OnChecked != null)
			{
				itemsChecked = true;
				checkedItems = new List<TreeViewItem>();

				foreach (string newlyCheckedItemKey in newlyCheckedItemKeys)
				{
					checkedItems.Add(lookupTable[newlyCheckedItemKey]);
				}
			}

			// Unchecked Items
			List<string> newlyUncheckedItemKeys = checkedItemCache.Where(x => !checkedItemKeys.Contains(x.Key) && x.Value).Select(x => x.Key).ToList();
			if (newlyUncheckedItemKeys.Any() && OnUnchecked != null)
			{
				itemsUnchecked = true;
				uncheckedItems = new List<TreeViewItem>();

				foreach (string newlyUncheckedItemKey in newlyUncheckedItemKeys)
				{
					uncheckedItems.Add(lookupTable[newlyUncheckedItemKey]);
				}
			}

			// Changed Items
			List<string> changedItemKeys = new List<string>();
			changedItemKeys.AddRange(newlyCheckedItemKeys);
			changedItemKeys.AddRange(newlyUncheckedItemKeys);
			if (changedItemKeys.Any() && OnChanged != null)
			{
				itemsChanged = true;
				changedItems = new List<TreeViewItem>();

				foreach (string changedItemKey in changedItemKeys)
				{
					changedItems.Add(lookupTable[changedItemKey]);
				}
			}

			// Persist states
			foreach (TreeViewItem item in lookupTable.Values)
			{
				item.IsChecked = checkedItemKeys.Contains(item.KeyValue);
				item.IsCollapsed = !expandedItemKeys.Contains(item.KeyValue);
			}

			UpdateItemCache();
		}

		/// <inheritdoc />
		protected internal override void RaiseResultEvents()
		{
			// Expanded items
			if (itemsExpanded && OnExpanded != null)
			{
				OnExpanded(this, new ExpandedEventArgs(expandedItems));
			}

			// Collapsed items
			if (itemsCollapsed && OnCollapsed != null)
			{
				OnCollapsed(this, new CollapsedEventArgs(collapsedItems));
			}

			// Checked items
			if (itemsChecked && OnChecked != null)
			{
				OnChecked(this, new CheckedEventArgs(checkedItems));
			}

			// Unchecked items
			if (itemsUnchecked && OnUnchecked != null)
			{
				OnUnchecked(this, new UncheckedEventArgs(uncheckedItems));
			}

			// Changed items
			if (itemsChanged && OnChanged != null)
			{
				OnChanged(this, new ChangedEventArgs(changedItems));
			}

			itemsExpanded = false;
			itemsCollapsed = false;
			itemsChecked = false;
			itemsUnchecked = false;
			itemsChanged = false;

			UpdateItemCache();
		}

		/// <summary>
		/// Provides data for the <see cref="Changed" /> event.
		/// </summary>
		public class ChangedEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ChangedEventArgs"/> class.
			/// </summary>
			/// <param name="changed">The items that have been changed.</param>
			public ChangedEventArgs(IReadOnlyCollection<TreeViewItem> changed)
			{
				Changed = changed;
			}

			/// <summary>
			/// Gets the items that have been changed.
			/// </summary>
			public IReadOnlyCollection<TreeViewItem> Changed { get; private set; }
		}

		/// <summary>
		/// Provides data for the <see cref="Expanded" /> event.
		/// </summary>
		public class ExpandedEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ExpandedEventArgs"/> class.
			/// </summary>
			/// <param name="expanded">The items that have been expanded.</param>
			public ExpandedEventArgs(IReadOnlyCollection<TreeViewItem> expanded)
			{
				Expanded = expanded;
			}

			/// <summary>
			/// Gets the items that have been expanded.
			/// </summary>
			public IReadOnlyCollection<TreeViewItem> Expanded { get; private set; }
		}

		/// <summary>
		/// Provides data for the <see cref="Collapsed" /> event.
		/// </summary>
		public class CollapsedEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="CollapsedEventArgs"/> class.
			/// </summary>
			/// <param name="collapsed">The items that have been collapsed.</param>
			public CollapsedEventArgs(IReadOnlyCollection<TreeViewItem> collapsed)
			{
				Collapsed = collapsed;
			}

			/// <summary>
			/// Gets the items that have been collapsed.
			/// </summary>
			public IReadOnlyCollection<TreeViewItem> Collapsed { get; private set; }
		}

		/// <summary>
		/// Provides data for the <see cref="Checked" /> event.
		/// </summary>
		public class CheckedEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="CheckedEventArgs"/> class.
			/// </summary>
			/// <param name="checked">The items that have been checked.</param>
			public CheckedEventArgs(IReadOnlyCollection<TreeViewItem> @checked)
			{
				Checked = @checked;
			}

			/// <summary>
			/// Gets the items that have been checked.
			/// </summary>
			public IReadOnlyCollection<TreeViewItem> Checked { get; private set; }
		}

		/// <summary>
		/// Provides data for the <see cref="Unchecked" /> event.
		/// </summary>
		public class UncheckedEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="UncheckedEventArgs"/> class.
			/// </summary>
			/// <param name="unchecked">The items that have been unchecked.</param>
			public UncheckedEventArgs(IReadOnlyCollection<TreeViewItem> @unchecked)
			{
				Unchecked = @unchecked;
			}

			/// <summary>
			/// Gets the items that have been unchecked.
			/// </summary>
			public IReadOnlyCollection<TreeViewItem> Unchecked { get; private set; }
		}
	}
}
