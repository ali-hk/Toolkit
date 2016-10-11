# Toolkit

A set of useful extensions, helpers, behaviors and controls for use in Windows UWP apps.
You can either cherry pick from the source or get NuGet packages for individual components, or the complete Toolkit.

A test app is also checked in with example usages of various UWP concepts and some of the components in Toolkit.

There is also a [Toolkit Template Pack][51] available in the Visual Studio Gallery (and from Extensions and Updates within Visual Studio), that provides a good starting point for writing a Prism + Toolkit-based blank app or Hamburger menu app. There are also some useful code snippets included.

### NuGet packages
| Component | Package | Version |
|-----------|---------|---------|
| Toolkit.Complete | [Toolkit.Complete][1] | [![21]][1] |
| Toolkit.Behaviors | [Toolkit.Behaviors][2] | [![22]][2] |
| Toolkit.Collections | [Toolkit.Collections][3] | [![23]][3] |
| Toolkit.Common | [Toolkit.Common][4] | [![24]][4] |
| Toolkit.Prism | [Toolkit.Prism][5] | [![25]][5] |
| Toolkit.Tasks | [Toolkit.Tasks][6] | [![26]][6] |
| Toolkit.Uwp | [Toolkit.Uwp][7] | [![27]][7] |
| Toolkit.Web | [Toolkit.Web.Uwp][8] | [![28]][8] |
| Toolkit.Xaml | [Toolkit.Xaml][9] | [![29]][9] |

## Toolkit.Complete
The complete Toolkit package including all of the below components, in one NuGet package.

## Toolkit.Behaviors
- **ItemClickCommandAction**: Powerful ItemClick command binding for ListView/GridView items. Executes a command on ListViewBase.ItemClick. The command can be bound or a path to the command can be specified.
- **DataTemplateSelectorBehavior**: Flexible DataTemplate selection with smart element caching for improved performance.
- **HighlightTextBehavior**: Highlight one or more occurences of a word in a TextBlock (color, underline, bold, italic, etc.)
- **HighlightListTextBehavior**: Highlight one or more occurences of a word in all TextBlocks in a ListView/GridView (color, underline, bold, italic, etc.)
- **RightTapMenuAction**: Easy right-click/right-tap context menus.
- **PagedScrollAction**: Action to allow a button click or similar event to cause a scrollable target to scroll in a given direction. Can be used for carousel effects.
- **KeyboardListScrollAction**: Customizable keyboard-shortcut ListView/GridView scrolling. Can be used with GamePad key-bindings for use on Xbox.
- **DismissParentFlyoutBehavior**: Dismiss a flyout from within the flyout with a behavior.
- **CustomCursorBehavior**: Easily declare a custom cursor to appear over a XAML element.
- **VisibilityOnFocusBehavior**: Make a XAML element visible when it's parent control has focus.

## Toolkit.Collections
- **DelegateComparer**: An easily reusable implementation of IComparer<T>, using a delegate Func.
- **FilteredObservableCollectionView**: An ObservableCollection view that can be filtered using a Predicate without breaking it's ObservableCollection behavior. Useful for easily filtering items in an ObservableCollection in the ViewModel while still automatically updating the View.
- **SortedObservableCollection**: An ObservableCollection that remains sorted based on a comparer Func.
- **VirtualizingCollection**: An implementation of ISupportIncrementalLoading and IItemsRangeInfo that allows easy data virtualization/Incremental loading for ListView/GridView.
- **VirtualizedDataSource**: Provides extended and advanced data virtualization support for ListView/GridView usage. Allows data virtualization/incremental loading for a ListView/GridView when scrolling in any direction (ListView:up, down; GridView: left, right).
- **EnumerableExtensions**: Extension methods for using IEnumerable, including: IsEmpty, IsNullOrEmpty, EmptyIfNull, Contains, ForEach, ForEach with async Actions, ForEach with async Funcs.
- **DictionaryExtensions**: Extension methods for retrieving keyed values from an IDictionary<string, object>, PropertySet or ValueSet
- **ItemIndexRangeExtensions**: Extension methods for using ItemIndexRange, including: Equals, ContiguousOrOverlaps, Intersects, Combine, DiffRanges and Overlap

## Toolkit.Common
- **InMemoryCache<TKey, TValue>**: A thread-safe in memory cache with specified max items and time to live or indefinite TTL. Supports in-place updating of items (via IUpdatable<T>) and detailed cache change information.
- **IUpdatable<T>**: A common interface for objects that can be updated in-place.
- **ScopedReaderWriterLock**: IDisposable support for ReaderWriterLockSlim for easy locking via using scopes.
- **WeakReferenceHelper**: Extension methods for using WeakReferences, including: AsWeakRef<T> for getting a WeakReference for any object, and SafeResolve<T> for safely resolving a WeakReference.
- **ReflectionHelper**: Extension methods for inspecting objects via reflection, including: GetPropertyFromPath which allows retrieval of a property value by specifying a path on an object.
- **EnumHelper**: Extension methods for using Enums, including: EnumFromString for safely parsing enum values from strings.
- **EventArgs<T>**: A generic implementation of EventArgs with a custom payload.
- **DoubleHelper**: Extension methods for handling Doubles with epsilons, including: IsZero, IsNaN, IsGreaterThan, IsLessThanOrClose, AreClose.
- **StringHelper**: Extension methods for using strings and string interpolation, including: InvariantCulture and CurrentCulture for culture-based string formatting when using string interpolation; and NotNullAndEquals, IsNullOrWhiteSpace and AllIndexesOf.
- **UrlEncodingHelper**: Extension methods for URL encoding, including: a correct URL encoder (UrlEncode).
- **CastingHelper**: Extension methods for safely casting object, including: SafeCast<T>.
- **TypeConverterHelper**: Extension methods for parsing and converting values, including: GetHexStringFromUint, GetBytesFromString, GetHexStringFromBytes, TryCreateUri, TryGetDateTimeValue and TryGetTimeSpanValue.

## Toolkit.Prism
- **MvvmPage**: An x:Bind friendly subclass of Prism's SessionStateAwarePage. This class exposes a strongly-typed DataContext (ConcreteDataContext) on the page for easy use with x:Bind.
- **MvvmUserControl**: An x:Bind friendly subclass of UserControl. This class exposes a strongly-typed DataContext (ConcreteDataContext) on the UserControl for easy use with x:Bind.
- **SessionStateHelper**: Extension methods for using Prism's SessionStateService, including: GetStateForKeyOrDefault<T> and TryGetStateForKey<T>.

## Toolkit.Tasks
- **TaskExtensions**: Extension methods for Task continuations and error handling, including: ObserveAndLogFailure, ObserveFailureWith, ContinueOnThisThreadWith, ObserveAndFailFast, GetSafeResult<T> and FailFastUnhandled.

## Toolkit.Uwp
- **DeviceTypeHelper**: A helper for detecting what type of device an app is running on (Desktop, Phone, Xbox, HoloLens) for easy feature enablement and device-specific code paths.
- **MemoryUsageHelper**: A helper for dumping total memory usage for use when optimizing memory performance on UWP devices.
- **UIThreadHelper**: A helper for checking if code is executing on the UI thread. Helpful for debugging wrong thread exceptions.
- **VersionHelper**: A helper for getting the running app's version information.

## Toolkit.Web
- **HttpClientExtensions**: Extension methods for safely calling web services via HttpClient. Throws exceptions with rich error details for easy debugging and better logging. Includes: SendRequestExtendedAsync, EnsureSuccessStatusCodeExtended and ToStringDetailed for HttpRequestMesssage and HttpResponseMessage with method, URI, headers and content.
- **HttpException**: An exception class providing rich and detailed exception information for HttpClient requests and responses for easier debugging and better logging than the standard COMException thrown by HttpClient which only has an HResult. Includes StatusCode, URI, headers and content.
- **QueryStringHelper**: Extension methods for easily parsing and creating query strings for HTTP web requests, including: ToQueryString from key-value pair, ToQueryString from Dictionary, ParseQueryString, GetQueryParmeter, TryGetUlongFromQueryParams, TryGetUintFromQueryParams and TryGetEnumFromQueryParams.
- **UriExtensions**: Extension methods for working URIs, including: AddQueryParam, WwwFormUrlDecoder.AsDictionary and ParseQueryString as WwwFormUrlDecoder.

## Toolkit.Xaml
- **Expander**: A control that encapsulates the behavior of a header view and a button expanding and collapsing a sub-view. Typical usage would be to provide accordion/drop-down functionality in a XAML-only, declarative fashion.
- **WrapPanel**: A panel for use with ItemsControl or ListView/GridView that positions child elements sequentially from left to right or top to bottom.  When elements extend beyond the panel edge, elements are positioned in the next row or column. A UWP port of the Silverlight Toolkit control, useful for showing a wrapping list of UI elements of different sizes.
- **BoolConditionalConverter**: A converter for specifying different values for a Binding based on a boolean value.
- **NullConditionalConverter**: A converter for specifying different values for a Binding if a value is null.
- **DispatcherHelper**: A helper for dispatching to and running work on the UI thread.
- **ElementLogger**: A helper for dumping the details of a UIElement, useful for debugging UI issues including Focus.
- **NavigationParameterHelper**: Extension methods for easily passing parameters and reading parameters from Navigation parameters.
- **DeviceTypeTrigger**: A VisualStateManager StateTrigger based on DeviceType for easily changing the layout or view based in XAML based on the device an app is running on.
- **ToggleableAdaptiveTrigger**: A VisualStateManager StateTrigger that behaves like AdaptiveTrigger but can be enabled and disabled as needed.
- **VisualTreeUtilities**: An extensive suite of Visual Tree utilities for walking and manipulating the Visual Tree, including: GetChild<T>, GetChildByName<T>, GetParent<T>, GetParentByName<T>, ContainsElement, ContainsElement<T>, ContainsElementOfType<T>, SetFocusOnChildControl, GetRootElement, GetFirstChildOfType<T>, ForEachChildOfType<T>, GetChildrenOfType<T>, GetFirstParentOfType<T> and GetAllFocusableControls.

## Toolkit.TestApp
Demonstrates usage of the following UWP concepts:
- DataTemplateSelectorBehavior, which showcases using ListViewBase.ContainerContentChanging and ListViewBase.ChoosingItemContainer to provide flexible DataTemplate selection behavior.
- DataTemplateSelector
- Incremental Loading, which showcases use of IncrementalUpdateBehavior from the XAML Behaviors SDK, x:Bind in combination with x:Phase and manual incremental loading with ListViewBase.ContainerContentChanging.
- x:DeferLoadStrategy, which showcases using x:DeferLoadStrategy="Lazy" to delay creation of XAML elements, which can improve load times. It also demonstrates how to realize deferred elements via FindName() in code-behind and VisualStateManager states.
- Customizing ListViewItemPresenter further in code-behind, which allows UWP apps to benefit from the reduced element count offered by ListViewItemPresenter while still allowing customization beyond the out-of-the-box properties.
- VisualStateManager StateTriggers, which showcases using AdaptiveTrigger and writing your own custom StateTriggers (for example to bind to a ViewModel property to cause a VisualState transition)
- Displaying heterogeneous data in a ListView or GridView using CollectionViewSource to group items and DataTemplateSelector to use different DataTemplates for each type of item
- XAML View MRT functionality, which allows developers to write different XAML files for each type of device (Desktop, Phone, Xbox, HoloLens, etc.) using the same code-behind and ViewModel
- SplitView for displaying Hamburger menus
- Using x:Bind with a ResourceDictionary

## Template Pack
Available from the Visual Studio Gallery and within Visual Studio from Tools -> Extensions and Updates -> Online, the Toolkit Template Pack includes:
- A project template for a Prism + Toolkit-based blank app
- A project template for a Prism + Toolkit-based Hamburger menu app
- An item template for creating a new Prism + Toolkit-based MvvmPage
- An item template for creating a new Prism + Toolkit-based PageViewModel
- Code snippets for quickly writing a DelegateCommand, async DelegateCommand, DelegateCommand with a parameter and a Bindable property.

[1]: https://www.nuget.org/packages/Toolkit.Complete/
[2]: https://www.nuget.org/packages/Toolkit.Behaviors/
[3]: https://www.nuget.org/packages/Toolkit.Collections/
[4]: https://www.nuget.org/packages/Toolkit.Common/
[5]: https://www.nuget.org/packages/Toolkit.Prism/
[6]: https://www.nuget.org/packages/Toolkit.Tasks/
[7]: https://www.nuget.org/packages/Toolkit.Uwp/
[8]: https://www.nuget.org/packages/Toolkit.Web.Uwp/
[9]: https://www.nuget.org/packages/Toolkit.Xaml/

[21]: https://img.shields.io/nuget/vpre/Toolkit.Complete.svg
[22]: https://img.shields.io/nuget/vpre/Toolkit.Behaviors.svg
[23]: https://img.shields.io/nuget/vpre/Toolkit.Collections.svg
[24]: https://img.shields.io/nuget/vpre/Toolkit.Common.svg
[25]: https://img.shields.io/nuget/vpre/Toolkit.Prism.svg
[26]: https://img.shields.io/nuget/vpre/Toolkit.Tasks.svg
[27]: https://img.shields.io/nuget/vpre/Toolkit.Uwp.svg
[28]: https://img.shields.io/nuget/vpre/Toolkit.Web.Uwp.svg
[29]: https://img.shields.io/nuget/vpre/Toolkit.Xaml.svg

[51]: https://visualstudiogallery.msdn.microsoft.com/5d7bbe5a-8eda-4363-bd0b-82daf6b05713