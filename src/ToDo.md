## Tasks
- [ ] Consolidate class naming (Helper vs. Utils vs. Extensions, etc.)
- [ ] Use extension methods where possible
- [X] Query String helper
- [X] FailFast
- [X] Converters
- [X] Behaviors
   - [X] Behavior<T>
   - [X] VisibilityOnFocusBehavior
   - [X] ItemClickCommandBehavior
   - [X] ExecuteCommandAction
   - [X] PagedScrollAction
   - [X] CustomCursorBehavior
   - [X] HighlightSearchTextBehavior
		- [X] Calculate newly visible range and only process that part
   - [X] RightTapMenuAction
- [ ] VariableSizeWrapGrid
	- [ ] Use a ViewModel-implemented interface to indicate weighting
- [X] IEnumerable IsEmpty
- [X] IEnumerable Contains(comparer)
- [X] IEnumerable ForEach
- [X] IEnumerable TrueForAll
- [X] Triggers
- [ ] Keyboard shortcut helper
- [ ] File helpers to make accessing app local files easy

- [X] Type-based DataTemplateSelector

- [X] Template implementation of:
	- [X] x:Bind (in page, datatemplate, resourcedic)
	- [X] x:Phase
	- [X] Incremental load behavior
		-https://msdn.microsoft.com/en-us/library/windows/apps/mt210946.aspx
- [X] Template impelmentation of ISupportIncrementalLoading
- [X] Template implemenation of:
	 - [X] CCC
	 - [X] CIC
- [X] Template implementation of IItemsRangeInfo
- [X] Template implementation of GoToElementStateCore for GVIP/LVIP
	https://code.msdn.microsoft.com/windowsapps/ListViewSimple-d5fc27dd/sourcecode?fileId=44767&pathId=248235732
- [X] Template implementation of CollectionViewSource
- [X] Example of x:DeferLoadStrategy
- [X] Example of VSM Triggers
	- [X] AdaptiveTrigger
	- [X] StateTrigger based off ViewModel property
- [X] Example of TemplatedControl

- [X] Template MRT usage
- [X] Rename VisualTree methods
- [X] ScopedReaderWriterLock extension methods
- [X] Update NuGet packages
- [ ] Add build flag to put things in System/Windows namespaces
- [ ] Add an expander sample page with Expanders as ListView items
- [X] Add an EnsureSuccessCode extension to HttpClient with rich exception info
- [ ] Add an EventSource for Toolkit
- [ ] Expander should have boolean for whether or not to set focus to first element on expand, in case there are no focusables
- [X] ValueSet helpers
- [X] Visual Tree find by name

- [ ] Create new project "XAML Performance Sample Suite"