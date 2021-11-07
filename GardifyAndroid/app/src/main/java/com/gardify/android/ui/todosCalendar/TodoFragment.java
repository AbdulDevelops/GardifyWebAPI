package com.gardify.android.ui.todosCalendar;

import android.content.Context;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.PopupMenu;
import android.widget.PopupWindow;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProviders;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.UserGarden.UserGarden;
import com.gardify.android.data.myGarden.UserPlant;
import com.gardify.android.data.todos.Diary;
import com.gardify.android.data.todos.EcoScan;
import com.gardify.android.data.todos.Todo;
import com.gardify.android.data.todos.TodoList;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.ui.todosCalendar.recyclerItems.CardTodoWithImage;
import com.gardify.android.ui.todosCalendar.recyclerItems.TodoDateCardItem;
import com.gardify.android.ui.todosCalendar.recyclerItems.TodoHeaderIcons;
import com.gardify.android.ui.generic.ExpandableHeaderTodoItem;
import com.gardify.android.ui.generic.recyclerItem.CardViewTopBottomSection;
import com.gardify.android.ui.generic.recyclerItem.HeaderTitle;
import com.gardify.android.ui.myGarden.MyGardenPersistDataViewModel;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.gardify.android.utils.TimeUtils;
import com.gardify.android.viewModelData.todos.TodoCalendarViewModel;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.json.JSONObject;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.stream.Collectors;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.APP_URL.isAndroid;
import static com.gardify.android.utils.TimeUtils.dateToString;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;


public class TodoFragment extends Fragment {
    private static final String TAG = "TodoFragment:";
    private static final String ARG_MY_GARDEN_TODO = "MY_GARDEN_TODO";

    public static final int TODO_TYPE_TODO = 1;
    public static final int TODO_TYPE_CUSTOM = 12;
    public static final int TODO_TYPE_ECO_SCAN= 20;
    public static final int TODO_TYPE_DIARY = 8;
    public static final int NON_CYCLIC_TODO = 0;
    public static final int ONE_MONTH_EARLIER = -1;
    public static final int EXACT_HOUR_VALUE = 22;
    public static final int IGNORE_HOST_NAME_CHARACTERS = 34;
    public static final int HEADER_SECTION = 2;
    public static final String API_DATE_PATTERN = "yyyy-MM-dd'T'HH:00:00'Z'";

    private RecyclerView recyclerView;
    private TextView noTodoMsgTextView;
    private ProgressBar progressBar;
    private List<TodoList> todoList;
    private List<Diary.ListEntry> diaryList;
    private List<EcoScan.ListEntry> ecoScanList;
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private String todoApiUrl, diaryApiUrl, bioScanApiUrl;
    private int selectedGardenId = 0;
    private int selectedGardenListSpinnerPosition = 0;

    // calendar
    private boolean showCalendar = false;
    private Calendar cal = Calendar.getInstance();
    // 0 represents current month
    private int calMonth = 0;
    private int calYear = 0;
    //bundle
    private String retrievedTodoString;
    private UserPlant.CyclicTodo retrievedTodo;

    // list to merge all todos
    List<TodoCalendarViewModel> todoCalendarViewModels = new ArrayList<>();

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            retrievedTodoString = getArguments().getString(ARG_MY_GARDEN_TODO);
            retrievedTodo = ApiUtils.getGsonParser().fromJson(retrievedTodoString, UserPlant.CyclicTodo.class);
            calMonth = Integer.parseInt(dateToString(retrievedTodo.getDateStart(), "yyyy-MM-dd'T'HH:00:00", "MM"));
            calYear = Integer.parseInt(dateToString(retrievedTodo.getDateStart(), "yyyy-MM-dd'T'HH:00:00", "yyyy"));
            // update calendar
            updateCalendar(calMonth, calYear);
        }
    }


    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_todo_main, container, false);

        init(root);

        //setup Toolbar
        setupToolbar(getActivity(), "TO-DO KALENDER", R.drawable.gardify_app_icon_to_do_kalender, R.color.toolbar_toDoCalendar_setupToolbar, true);


        if (getArguments() == null) {
            calMonth = Integer.parseInt(dateToString(generateStartDate(ToDoType.diary), "yyyy-MM-dd'T'HH:00:00", "MM"));
            calYear = Integer.parseInt(dateToString(generateStartDate(ToDoType.diary), "yyyy-MM-dd'T'HH:00:00", "yyyy"));
        }

        CallDiaryAPI(calMonth, calYear);

        return root;
    }

    public void init(View root) {
        /* finding views block */
        recyclerView = root.findViewById(R.id.recyclerView_todo);
        progressBar = root.findViewById(R.id.progressBar_todo);
        noTodoMsgTextView = root.findViewById(R.id.text_view_no_todos_message);
    }

    private void onSuccessDiary(Diary model, RequestData data) {

        clearLists();

        diaryList = new ArrayList<>();
        diaryList = model.getListEntries();

        if (diaryList != null) {
            for (Diary.ListEntry diary : diaryList) {
                String imageUrl = diary.getEntryImages().size() > 0 ? diary.getEntryImages().get(0).getSrcAttr() : null;
                if (imageUrl != null)
                    // ignores the url part C:\\Webs\\gardifybackend.sslbeta.de\\
                    imageUrl = imageUrl.substring(IGNORE_HOST_NAME_CHARACTERS);

                TodoCalendarViewModel todoDiary = new TodoCalendarViewModel()
                        .setId(diary.getId())
                        .setCyclicId(NON_CYCLIC_TODO)
                        .setReferenceType(TODO_TYPE_DIARY)
                        .setDate(diary.getDate())
                        .setTitle(diary.getTitle())
                        .setDescription(diary.getDescription())
                        .setImageUrl(imageUrl)
                       // .setNotes(diary.get) //TODO add note to diary (waiting for api)
                        .setFinished(false)
                        .setDeleted(false)
                        .setIgnored(false)
                        .build();

                todoCalendarViewModels.add(todoDiary);
            }
        }
        sortTodosAscending();

        InitializeGroupAdapter();

    }

    private void onSuccessTodo(Todo model, RequestData data) {

        todoList = new ArrayList<>();
        todoList = model.getTodoList();

        for (TodoList todos : todoList) {
            String imageUrl = todos.getEntryImages().size() > 0 ? todos.getEntryImages().get(0).getSrcAttr() : null;

            TodoCalendarViewModel todoDefault = new TodoCalendarViewModel()
                    .setId(todos.getId())
                    .setDate(todos.getDateStart())
                    .setDateStart(todos.getDateStart())
                    .setDateEnd(todos.getDateEnd())
                    .setCyclicId(todos.getCyclicId() != null ? todos.getCyclicId() : 0)
                    .setReferenceType(todos.getReferenceType())
                    .setTitle(todos.getTitle())
                    .setDescription(todos.getDescription())
                    .setNotes(todos.getNotes())
                    .setImageUrl(imageUrl)
                    .setDeleted(todos.getDeleted())
                    .setFinished(todos.getFinished())
                    .setIgnored(todos.getIgnored()).build();

            todoCalendarViewModels.add(todoDefault);
        }
        CallEcoScanAPI(calMonth, calYear);
    }

    private void onSuccessEcoScan(EcoScan model, RequestData data) {
        ecoScanList = new ArrayList<>();
        ecoScanList = model.getListEntries();

        if (ecoScanList != null) {
            for (EcoScan.ListEntry diary : ecoScanList) {

                TodoCalendarViewModel todoDiary = new TodoCalendarViewModel()
                        .setId(diary.getId())
                        .setCyclicId(NON_CYCLIC_TODO)
                        .setReferenceType(TODO_TYPE_ECO_SCAN)
                        .setDate(diary.getDate())
                        .setTitle(diary.getTitle())
                        .setDescription(diary.getDescription())
                        .setImageUrl("")
                        .setFinished(false)
                        .setDeleted(false)
                        .setIgnored(false)
                        .build();

                todoCalendarViewModels.add(todoDiary);
            }
        }
        progressBar.setVisibility(View.GONE);

        sortTodosAscending();

        updateAllTodos();
    }

    private void InitializeGroupAdapter() {

        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        populateAdapter();
        recyclerView.setAdapter(groupAdapter);
    }


    private Section headerDateNavigation;
    private Section calendarDateSection, cardViewTopSection, cardViewBottomSection;

    private void populateAdapter() {
        groupAdapter.clear();

        Section headerGridList = new Section(new TodoHeaderIcons("", getContext(), selectedGardenListSpinnerPosition, R.drawable.gardify_info_icon,
                R.drawable.gardify_to_do_kalender_on, R.drawable.gardify_to_do_plus_icon, onIconClickListener));
        groupAdapter.add(headerGridList);

        cardViewTopSection = new Section(new CardViewTopBottomSection(true));
        cardViewBottomSection = new Section(new CardViewTopBottomSection(false));

        calendarDateSection = new Section();
        headerDateNavigation = new Section(new TodoDateCardItem(getContext(),0, todosSortedByDay,
                calMonth, calYear, false, onDateNavCardClickListener));
        calendarDateSection.add(headerDateNavigation);
        groupAdapter.add(calendarDateSection);

        updateAllTodos();
        CallTodoAPI();

    }

    Map<String, List<TodoCalendarViewModel>> todosSortedByDay;
    Map<String, List<TodoCalendarViewModel>> todosSortedByDayUnchanged;
    private void sortTodosAscending() {
        todosSortedByDay = new HashMap<>();

        // remove todos other than selected month
        todoCalendarViewModels = todoCalendarViewModels.stream().filter(x ->
                Integer.parseInt(TimeUtils.dateToString(x.getDate(),
                        "yyyy-MM-dd'T'HH:00:00", "MM")) == calMonth)
                .collect(Collectors.toList());

        // Group by Date
        todosSortedByDay = todoCalendarViewModels.stream().collect(Collectors.groupingBy(element
                -> element.getDate()));

        // sort date in ascending order
        todosSortedByDay = todosSortedByDay.entrySet().stream()
                .sorted(Map.Entry.comparingByKey())
                .collect(Collectors.toMap(Map.Entry::getKey, Map.Entry::getValue,
                        (oldValue, newValue) -> oldValue, LinkedHashMap::new));




        todosSortedByDayUnchanged = todosSortedByDay;
    }

    //todosSection = new Section();
    List<Section> todosSectionRows = null;

    private void updateAllTodos() {

        // if section already exists remove it
        if (groupAdapter.getItemCount() > HEADER_SECTION) {
            groupAdapter.removeAll(todosSectionRows);
        }

        //NoTodo Message
        if (todosSortedByDay.size() > 0) {
            noTodoMsgTextView.setVisibility(View.GONE);
        } else {
            noTodoMsgTextView.setVisibility(View.VISIBLE);
        }
        todosSectionRows = new ArrayList<>();
        for (Map.Entry<String, List<TodoCalendarViewModel>> entry : todosSortedByDay.entrySet()) {
            String key = entry.getKey();
            Section headerDate = new Section(new HeaderTitle("ab " + dateToString(key, "yyyy-MM-dd'T'HH:00:00", "EEE dd.MM"), true));
            todosSectionRows.add(headerDate);
            List<TodoCalendarViewModel> todoDayList = entry.getValue();

            // Todos item with top and bottom section
            Section sectionWithBackground = new Section();
            sectionWithBackground.setHeader(cardViewTopSection);
            sectionWithBackground.setFooter(cardViewBottomSection);

            for (TodoCalendarViewModel todosVM : todoDayList) {
                ToDoType toDoType = getTodoBasedOnReference(todosVM.getReferenceType());
                ExpandableHeaderTodoItem expandableHeaderList = new ExpandableHeaderTodoItem(getContext(), getTodoColor(toDoType),
                        R.color.text_all_gunmetal, R.color.expandableHeader_all_white, todosVM, 0, onExpandableHeaderListener);
                ExpandableGroup expandableGroupListen = new ExpandableGroup(expandableHeaderList);
                expandableGroupListen.add(new CardTodoWithImage(getContext(), R.color.expandableGroup_all_whiteSmoke, todosVM));
                sectionWithBackground.add(expandableGroupListen);
            }
            todosSectionRows.add(sectionWithBackground);
        }
        //todosSection.addAll(todosSectionRows);
        groupAdapter.addAll(todosSectionRows);
    }

    private TodoDateCardItem.OnDateNavCardClickListener onDateNavCardClickListener = (viewBinding, view, selectedYear, selectedMonth, selectedCalendarDay) -> {
        if (viewBinding.calendarView.equals(view)) {

            Date selectedDate = selectedCalendarDay.getDate();
            DateFormat outFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:00:00", Locale.ENGLISH);
            String dateString = outFormat.format(selectedDate);

            // reset sortedTodo to default
            todosSortedByDay = todosSortedByDayUnchanged;

            // filter sortedTodo list as per selected date

            todosSortedByDay = todosSortedByDay.entrySet()
                    .stream()
                    .filter(map -> map.getKey().equals(dateString))
                    .collect(Collectors.toMap(map -> map.getKey(), map -> map.getValue()));


            updateAllTodos();

        } else {
            updateCalendar(selectedMonth+1, selectedYear);
            //Refresh all list
            CallDiaryAPI(calMonth, calYear);
        }


    };

    private ExpandableHeaderTodoItem.OnExpandableHeaderListener onExpandableHeaderListener = (todos, binding, view) -> {
        if (binding.todoOptions.equals(view)) {
            showTodoMenuOptions(binding.todoOptions, todos);
        } else if (binding.todoCheckbox.equals(view)){
            if(todos.isFinished()){
                // marks as true
                String markTodoFalseApi = APP_URL.TODO_API + "markfinished/false/"+ todos.getId()+"/"+ todos.getCyclicId();
                RequestQueueSingleton.getInstance(getContext()).objectRequest(markTodoFalseApi, Request.Method.PUT, this::onSuccessTodoCheckBoxUpdate, null, null);
            } else {
                String markTodoTrueApi = APP_URL.TODO_API + "markfinished/true/"+ todos.getId()+"/"+ todos.getCyclicId();
                RequestQueueSingleton.getInstance(getContext()).objectRequest(markTodoTrueApi, Request.Method.PUT, this::onSuccessTodoCheckBoxUpdate, this::onErrorTodoCheckBoxUpdate, null);
            }
        }
    };

    private void onErrorTodoCheckBoxUpdate(VolleyError volleyError) {
        Toast.makeText(getContext(), volleyError.getMessage(), Toast.LENGTH_SHORT).show();
    }

    private void onSuccessTodoCheckBoxUpdate(JSONObject jsonObject) {
        Log.d(TAG, "todo checkbox updated successfully");
    }

    private TodoHeaderIcons.onIconClickListener onIconClickListener = (imageId, viewBinding, view, _spinnerPosition) -> {

        if (viewBinding.spinnerTodoFragment.equals(view)) {
            UserGarden selectedGarden = (UserGarden) viewBinding.spinnerTodoFragment.getSelectedItem();
            selectedGardenId = selectedGarden.getId();
            selectedGardenListSpinnerPosition = _spinnerPosition;
            //Refresh all list
            CallDiaryAPI(calMonth, calYear);
        }

        switch (imageId) {
            case R.id.spinner_todo_fragment:

                break;
            case R.drawable.gardify_info_icon:
                displayInfoOptionMenu(view);

                break;
            case R.drawable.gardify_to_do_kalender_on:

                showCalendar = !showCalendar;

                calendarDateSection.remove(headerDateNavigation);
                headerDateNavigation = new Section(new TodoDateCardItem(getContext(), 0, todosSortedByDay,
                        calMonth, calYear, showCalendar, onDateNavCardClickListener));
                calendarDateSection.add(headerDateNavigation);

                break;
            case R.drawable.gardify_to_do_plus_icon:
                displayAddOptionMenu(view);

                break;
        }
    };

    public void showTodoMenuOptions(View v, TodoCalendarViewModel todos) {
        PopupMenu popup = new PopupMenu(getContext(), v);
        popup.inflate(R.menu.menu_todo_itemoption);
        popup.setOnMenuItemClickListener(item -> {
            switch (item.getItemId()) {
                case R.id.todo_menu_edit:
                    // read the listItemPosition here

                    Bundle args = new Bundle();
                    String todoJsonString = ApiUtils.getGsonParser().toJson(todos);

                    switch (getTodoBasedOnReference(todos.getReferenceType())) {
                        case todo:
                        case custom:
                            args.putString("TODO_CUSTOM", todoJsonString);
                            // show customTodo bottomSheet
                            AddTodoBottomSheetDialog editCustomTodoBottomSheet = new AddTodoBottomSheetDialog();
                            editCustomTodoBottomSheet.setArguments(args);
                            editCustomTodoBottomSheet.setCancelable(false);
                            editCustomTodoBottomSheet.show((getActivity()).getSupportFragmentManager(),
                                    "ModalBottomSheet");

                            popup.dismiss();
                            break;
                        case diary:
                            args.putString("TODO_DIARY", todoJsonString);
                            AddDiaryBottomSheetDialog editDiaryBottomSheetFragment = new AddDiaryBottomSheetDialog();
                            editDiaryBottomSheetFragment.setArguments(args);
                            editDiaryBottomSheetFragment.setCancelable(false);
                            editDiaryBottomSheetFragment.show((getActivity()).getSupportFragmentManager(),
                                    "ModalBottomSheet");
                            popup.dismiss();
                            break;
                        case scan:
                            navigateToFragment(R.id.nav_controller_eco_scan, getActivity(), false, null);
                            break;
                    }

                    return true;
                case R.id.todo_menu_delete:

                    generateDeleteDialogs(todos);

                    return true;
                default:
                    return false;
            }
        });
        popup.show();
    }

    private void generateDeleteDialogs(TodoCalendarViewModel todos) {
        String deleteTodoUrl = getDeleteUrl(todos);

        if (todos.getCyclicId()!= NON_CYCLIC_TODO) {
            new GenericDialog.Builder(getContext())
                    .setTitle("Bist du sicher, dass du dieses To-do löschen möchtest?")
                    .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                    .addNewButton(R.style.PrimaryWarningButtonStyle,
                            "Diesen Termin löschen",R.dimen.textSize_body_medium, view -> {
                        String modifiedUrl = deleteTodoUrl + "/false";
                        executeDeleteRequest(modifiedUrl);
                    })
                    .addNewButton(R.style.PrimaryWarningButtonStyle,
                            "Alle Termine löschen", R.dimen.textSize_body_medium, view -> {
                        String modifiedUrl = deleteTodoUrl + "/true";
                        executeDeleteRequest(modifiedUrl);
                    })
                    .addNewButton(R.style.SecondaryButtonStyle,
                            "Abbrechen", R.dimen.textSize_body_medium, view ->
                            Log.e(TAG, "dialog dismissed."))
                    .setButtonOrientation(LinearLayout.VERTICAL)
                    .setCancelable(true)
                    .generate();
        } else {
            new GenericDialog.Builder(getContext())
                    .setTitle("Bist du sicher, dass du dieses To-do löschen möchtest?")
                    .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                    .addNewButton(R.style.PrimaryWarningButtonStyle,
                            "Diesen Termin löschen", R.dimen.textSize_body_medium, view -> {
                                ToDoType toDoType = getTodoBasedOnReference(todos.getReferenceType());
                                String modifiedUrl;
                                if(toDoType==ToDoType.custom){
                                     modifiedUrl = deleteTodoUrl + "/false";
                                }  else {
                                     modifiedUrl = deleteTodoUrl;
                                }
                                executeDeleteRequest(modifiedUrl);
                            })
                    .addNewButton(R.style.SecondaryButtonStyle,
                            "Abbrechen", R.dimen.textSize_body_medium, view ->
                            Log.e(TAG, "dialog dismissed."))
                    .setButtonOrientation(LinearLayout.VERTICAL)
                    .setCancelable(true)
                    .generate();
        }
    }
    private String getDeleteUrl(TodoCalendarViewModel _todos) {
        TodoFragment.ToDoType toDoType = getTodoBasedOnReference(_todos.getReferenceType());
        String deleteUrl = "";
        switch (toDoType) {
            case scan:
            case diary:
                deleteUrl = APP_URL.DIARY_API + _todos.getId();
                break;
            case custom:
            case todo:
                deleteUrl = APP_URL.TODO_API + _todos.getId();
                break;
        }
        return deleteUrl;
    }

    private void executeDeleteRequest(String modifiedUrl) {
        RequestQueueSingleton.getInstance(getContext()).objectRequest(modifiedUrl, Request.Method.DELETE, this::TodoDeleteSuccess, this::OnError, null);
    }

    private void TodoDeleteSuccess(JSONObject jsonObject) {

        displayAlertDialog(getContext(), "Todo wurde erfolgreich gelöscht");
        groupAdapter.clear();

        //Refresh all list
        CallDiaryAPI(calMonth, calYear);

    }

    private void OnError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
    }

    public void displayInfoOptionMenu(View v) {


        LayoutInflater layoutInflater = (LayoutInflater) getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        final View popupView = layoutInflater.inflate(R.layout.popup_menu_todo_frag_info, null);

        PopupWindow popupWindow = new PopupWindow(
                popupView,
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);

        popupWindow.setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
        popupWindow.setElevation(20);
        popupWindow.setOutsideTouchable(true);


        popupWindow.showAsDropDown(v, -20, 0);
    }

    private void clearLists() {
        if (todosSortedByDay != null) {
            todosSortedByDay.clear();
        }
        if (todoCalendarViewModels.size() > 0) {
            todoCalendarViewModels.clear();
        }
    }

    public void displayAddOptionMenu(View v) {


        LayoutInflater layoutInflater = (LayoutInflater) getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        final View popupView = layoutInflater.inflate(R.layout.popup_menu_todo_add, null);

        PopupWindow popupWindow = new PopupWindow(
                popupView,
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);

        popupWindow.setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
        popupWindow.setElevation(20);
        popupWindow.setOutsideTouchable(true);

        TextView customTodoButton = popupView.findViewById(R.id.text_view_todo_popup_custom_todo);
        TextView diaryTodoButton = popupView.findViewById(R.id.text_view_todo_popup_diary_todo);
        TextView addEcoScanButton = popupView.findViewById(R.id.text_view_todo_popup_eco_scan);

        customTodoButton.setOnClickListener(v1 -> {
            // show customTodo bottomSheet
            AddTodoBottomSheetDialog addCustomTodoBottomSheet = new AddTodoBottomSheetDialog();
            addCustomTodoBottomSheet.setCancelable(false);
            addCustomTodoBottomSheet.show((getActivity()).getSupportFragmentManager(),
                    "ModalBottomSheet");

            // dismiss popupWindow
            popupWindow.dismiss();
        });

        diaryTodoButton.setOnClickListener(v1 -> {
            // show diaryTodo bottomSheet
            AddDiaryBottomSheetDialog addDiaryBottomSheetFragment = new AddDiaryBottomSheetDialog();
            addDiaryBottomSheetFragment.setCancelable(false);
            addDiaryBottomSheetFragment.show((getActivity()).getSupportFragmentManager(),
                    "ModalBottomSheet");

            // dismiss popupWindow
            popupWindow.dismiss();
        });

        addEcoScanButton.setOnClickListener(v1 -> {
            navigateToFragment(R.id.nav_controller_eco_scan, getActivity(), false, null);
            // dismiss popupWindow
            popupWindow.dismiss();
        });

        popupWindow.showAsDropDown(v, -20, 0);
    }

    private void CallTodoAPI() {
        todoApiUrl = APP_URL.TODO_API + isAndroid() + "&period=month&startDate=" + generateStartDate(ToDoType.todo) + "&gid=" + selectedGardenId;
        RequestQueueSingleton.getInstance(getContext()).typedRequest(todoApiUrl, this::onSuccessTodo, null, Todo.class, new RequestData(RequestType.Todo));
    }

    private void CallDiaryAPI(int month, int year) {
        diaryApiUrl = APP_URL.DIARY_API + "?m=" + month + "&y=" + year;
        RequestQueueSingleton.getInstance(getContext()).typedRequest(diaryApiUrl, this::onSuccessDiary, null, Diary.class, new RequestData(RequestType.Diary));
        progressBar.setVisibility(View.VISIBLE);
    }

    private void CallEcoScanAPI(int month, int year) {
        bioScanApiUrl = APP_URL.DIARY_API + "BioScan?m=" + month + "&y=" + year;
        RequestQueueSingleton.getInstance(getContext()).typedRequest(bioScanApiUrl, this::onSuccessEcoScan, null, EcoScan.class, new RequestData(RequestType.EcoScan));
    }

    public String generateStartDate(ToDoType toDoType) {
        Date calDateTime = null;
        switch (toDoType) {
            case todo:
                cal.add(Calendar.MONTH, ONE_MONTH_EARLIER); // for custom todos get last day of prev month
                cal.set(Calendar.DAY_OF_MONTH, cal.getActualMaximum(Calendar.DAY_OF_MONTH));
                cal.set(Calendar.HOUR_OF_DAY, EXACT_HOUR_VALUE);
                calDateTime = cal.getTime();
                cal.add(Calendar.MONTH, +1); //reset month

                break;
            case diary:
                cal.set(Calendar.DAY_OF_MONTH, cal.getActualMinimum(Calendar.DAY_OF_MONTH));
                calDateTime = cal.getTime();
                break;
        }


        return new SimpleDateFormat(API_DATE_PATTERN).format(calDateTime);
    }

    public void updateCalendar(int month, int year) {

        String formattedDate = new SimpleDateFormat(API_DATE_PATTERN).format(cal.getTime()); // format date

        Log.d("TODO: calendar before update", "" + formattedDate);

        cal.set(Calendar.MONTH, month-1); // cal month start with 0
        cal.set(Calendar.DAY_OF_MONTH, 1);
        cal.set(Calendar.YEAR, year);

        String formattedDateUpdate = new SimpleDateFormat(API_DATE_PATTERN).format(cal.getTime()); // format date
        int updatedMonth = Integer.parseInt(new SimpleDateFormat("MM").format(cal.getTime())); // format date
        int updatedYear = Integer.parseInt(new SimpleDateFormat("yyyy").format(cal.getTime())); // format date

        calMonth= updatedMonth;
        calYear= updatedYear;

        Log.d("TODO: calendar after update", "" + formattedDateUpdate);
    }

    public enum ToDoType {
        todo,
        custom,
        diary,
        scan,
    }

    public static ToDoType getTodoBasedOnReference(int referenceType) {
        switch (referenceType) {
            case TODO_TYPE_TODO:
                return ToDoType.todo;
            case TODO_TYPE_DIARY:
                return ToDoType.diary;
            case TODO_TYPE_ECO_SCAN:
                return ToDoType.scan;
            case TODO_TYPE_CUSTOM:
                return ToDoType.custom;
        }
        return null;
    }

    int getTodoColor(ToDoType toDoType) {
        switch (toDoType) {
            case todo:
                return R.color.expandableHeader_todoCalendar_toDo;
            case custom:
                return R.color.expandableHeader_todoCalendar_ownToDo;
            case diary:
                return R.color.expandableHeader_todoCalendar_diaryEntry;
            case scan:
                return R.color.expandableHeader_todoCalendar_ecoScan;
            default:
                return R.color.expandableHeader_todoCalendar_dawn;
        }
    }


    MyGardenPersistDataViewModel persistDataViewModel;
    @Override
    public void onActivityCreated(@Nullable Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        persistDataViewModel = ViewModelProviders.of(getActivity()).get(MyGardenPersistDataViewModel.class);
        persistDataViewModel.setMyGardenState(R.string.all_calendar);
    }

    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     */
    @Override
    public void onResume() {
        super.onResume();

        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, getActivity(), true, null);
        }
    }


}