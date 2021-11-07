package com.gardify.android.ui.todosCalendar.recyclerItems;

import android.content.Context;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.ui.todosCalendar.calendarDecorator.EventDecorator;
import com.gardify.android.ui.todosCalendar.TodoFragment;
import com.gardify.android.viewModelData.todos.TodoCalendarViewModel;
import com.gardify.android.databinding.RecyclerItemTodoDateCardBinding;
import com.prolificinteractive.materialcalendarview.CalendarDay;
import com.prolificinteractive.materialcalendarview.format.ArrayWeekDayFormatter;
import com.xwray.groupie.databinding.BindableItem;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;
import static com.gardify.android.ui.todosCalendar.TodoFragment.getTodoBasedOnReference;

public class TodoDateCardItem extends BindableItem<RecyclerItemTodoDateCardBinding> {

    private int bgColor;
    private Context context;
    private OnDateNavCardClickListener onDateNavCardClickListener;
    private String startDate;
    private boolean showCalendar;
    private Map<String, List<TodoCalendarViewModel>> todosSortedByDay;
    private RecyclerItemTodoDateCardBinding viewBinding;
    private int mMonth;
    private Boolean yearSpinnerTouched = false;
    private Boolean monthSpinnerTouched = false;

    String selectedParsedMonth;
    int selectedParsedYear;

    List<String> monthsArray = Arrays.asList("Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember");
    List<Integer> yearsArray;

    // ArrayLists for day Decorator
    ArrayList<CalendarDay> allDays = new ArrayList<>();
    ArrayList<CalendarDay> todoDefaultDays = new ArrayList<>();
    ArrayList<CalendarDay> todoCustomDays = new ArrayList<>();
    ArrayList<CalendarDay> todoDiaryDays = new ArrayList<>();
    ArrayList<CalendarDay> todoScanDays = new ArrayList<>();

    public TodoDateCardItem(Context context, int bgColor, Map<String, List<TodoCalendarViewModel>> todosSortedByDay,
                            int mMonth, int selectedParsedYear, boolean showCalendar, OnDateNavCardClickListener onDateNavCardClickListener) {
        this.bgColor = bgColor;
        this.context = context;
        this.todosSortedByDay = todosSortedByDay;
        this.showCalendar = showCalendar;
        this.mMonth = mMonth;
        this.selectedParsedYear = selectedParsedYear;
        this.onDateNavCardClickListener = onDateNavCardClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_item_todo_date_card;
    }

    @Override
    public void bind(@NonNull RecyclerItemTodoDateCardBinding binding, int position) {
        viewBinding = binding;
        viewBinding.calendarView.setVisibility(View.GONE);

        // spinner month and year values
        selectedParsedMonth = monthsArray.get(mMonth-1); // array starts from 0 hence -1
        setupMonthSpinner(selectedParsedMonth);
        setupYearSpinner(selectedParsedYear);

        //Onclick listeners for currentParsedMonth currentYear buttons
        viewBinding.monthPrev.setOnClickListener(v -> onDateNavCardClickListener.onClick(binding, binding.monthPrev, selectedParsedYear, monthsArray.indexOf(selectedParsedMonth) - 1, null));
        viewBinding.monthNext.setOnClickListener(v -> onDateNavCardClickListener.onClick(binding, binding.monthNext, selectedParsedYear, monthsArray.indexOf(selectedParsedMonth) + 1, null));
        viewBinding.yearPrev.setOnClickListener(v -> onDateNavCardClickListener.onClick(binding, binding.yearPrev, selectedParsedYear - 1, monthsArray.indexOf(selectedParsedMonth), null));
        viewBinding.yearNext.setOnClickListener(v -> onDateNavCardClickListener.onClick(binding, binding.yearNext, selectedParsedYear + 1, monthsArray.indexOf(selectedParsedMonth), null));


        if (showCalendar) {
            viewBinding.calendarView.setVisibility(View.VISIBLE);
            viewBinding.calendarView.setTopbarVisible(false);
            viewBinding.calendarView.setWeekDayFormatter(new ArrayWeekDayFormatter(context.getResources().getTextArray(R.array.toDoCalendar_customWeekdaysGermanArray)));
            // viewBinding.calendarView.addDecorators(new WeekDayBackgroundDecorator(context));
            // add events
            for (Map.Entry<String, List<TodoCalendarViewModel>> entry : todosSortedByDay.entrySet()) {
                String key = entry.getKey();
                Date date = convertToDate(key);
                CalendarDay day = CalendarDay.from(date);
                List<TodoCalendarViewModel> calendarViewModelList = entry.getValue();
                for (TodoCalendarViewModel todoCalendarViewModel : calendarViewModelList) {
                    SeparateTodoTypesInList(getTodoBasedOnReference(todoCalendarViewModel.getReferenceType()), day);
                }
                allDays.add(day);
            }
            if (allDays.size() > 0)
                viewBinding.calendarView.setCurrentDate(allDays.get(0));

            viewBinding.calendarView.addDecorator(new EventDecorator(context, R.color.calendarView_todoCalendar_toDo, todoDefaultDays, TodoFragment.ToDoType.todo));
            viewBinding.calendarView.addDecorator(new EventDecorator(context, R.color.calendarView_todoCalendar_ownToDo, todoCustomDays, TodoFragment.ToDoType.custom));
            viewBinding.calendarView.addDecorator(new EventDecorator(context, R.color.calendarView_todoCalendar_diaryEntry, todoDiaryDays, TodoFragment.ToDoType.diary));
            viewBinding.calendarView.addDecorator(new EventDecorator(context, R.color.calendarView_todoCalendar_ecoScan, todoScanDays, TodoFragment.ToDoType.scan));

            viewBinding.calendarView.setOnDateChangedListener((widget, date, selected) -> onDateNavCardClickListener.onClick(viewBinding, viewBinding.calendarView, selectedParsedYear + 1, monthsArray.indexOf(selectedParsedMonth), date));

        }
    }

    private void setupMonthSpinner(String month) {


        ArrayAdapter<String> monthArrayAdapter = new ArrayAdapter<>(context, R.layout.custom_spinner_item, monthsArray); //selected item will look like a spinner set from XML
        viewBinding.spinnerTodoDateMonth.setAdapter(monthArrayAdapter);
        viewBinding.spinnerTodoDateMonth.setSelection(monthsArray.indexOf(month));

        // set touch and click listener for spinner
        viewBinding.spinnerTodoDateMonth.setOnItemSelectedListener(onMonthSelected);
        viewBinding.spinnerTodoDateMonth.setOnTouchListener((v, event) -> {
            monthSpinnerTouched = true;
            return false;
        });
    }

    private void setupYearSpinner(int year) {

        yearsArray = RangeYear(String.valueOf(year - 5), String.valueOf(year + 5));
        ArrayAdapter<Integer> yearArrayAdapter = new ArrayAdapter<>(context, R.layout.custom_spinner_item, yearsArray); //selected item will look like a spinner set from XML
        viewBinding.spinnerTodoDateYear.setAdapter(yearArrayAdapter);
        viewBinding.spinnerTodoDateYear.setSelection(yearsArray.indexOf(year));

        // set touch and click listener for spinner
        viewBinding.spinnerTodoDateYear.setOnItemSelectedListener(onYearSelected);
        viewBinding.spinnerTodoDateYear.setOnTouchListener((v, event) -> {
            yearSpinnerTouched = true;
            return false;
        });
    }

    public AdapterView.OnItemSelectedListener onYearSelected = new AdapterView.OnItemSelectedListener() {
        @Override
        public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
            // 0 is default position, which doesn't trigger onclick
            if (yearSpinnerTouched) {
                yearSpinnerTouched = false;
                int _year = yearsArray.get(position);
                int _month = monthsArray.indexOf(selectedParsedMonth);
                onDateNavCardClickListener.onClick(viewBinding, viewBinding.spinnerTodoDateYear, _year, _month, null);
            }
        }

        @Override
        public void onNothingSelected(AdapterView<?> parent) {
        }
    };

    public AdapterView.OnItemSelectedListener onMonthSelected = new AdapterView.OnItemSelectedListener() {
        @Override
        public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
            // 0 is default position, which doesn't trigger onclick
            if (monthSpinnerTouched) {
                monthSpinnerTouched = false;
                int _month = position;
                onDateNavCardClickListener.onClick(viewBinding, viewBinding.spinnerTodoDateYear, selectedParsedYear, _month, null);
            }
        }

        @Override
        public void onNothingSelected(AdapterView<?> parent) {
        }
    };

    private Date convertToDate(String key) {
        DateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:00:00", Locale.ENGLISH);
        Date date = null;
        try {
            date = format.parse(key);
        } catch (ParseException e) {
            e.printStackTrace();
        }
        return date;
    }

    public static List<Integer> RangeYear(String startYear, String endYear) {
        return IntStream.rangeClosed
                (Integer.parseInt(Objects.requireNonNull(startYear)),
                        Integer.parseInt(Objects.requireNonNull(endYear)))
                .boxed()
                .collect(Collectors.toList());
    }

    public interface OnDateNavCardClickListener {
        void onClick(RecyclerItemTodoDateCardBinding viewBinding, View view, int year, int month, CalendarDay selectedCalendarDay);
    }

    void SeparateTodoTypesInList(TodoFragment.ToDoType toDoType, CalendarDay day) {
        switch (toDoType) {
            case todo:
                todoDefaultDays.add(day);
                break;
            case custom:
                todoCustomDays.add(day);
                break;
            case diary:
                todoDiaryDays.add(day);
                break;
            case scan:
                todoScanDays.add(day);
                break;
        }
    }
}
