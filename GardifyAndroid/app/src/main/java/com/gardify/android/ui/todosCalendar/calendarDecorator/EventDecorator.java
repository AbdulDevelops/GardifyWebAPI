package com.gardify.android.ui.todosCalendar.calendarDecorator;

import android.content.Context;

import com.gardify.android.ui.todosCalendar.TodoFragment;
import com.prolificinteractive.materialcalendarview.CalendarDay;
import com.prolificinteractive.materialcalendarview.DayViewDecorator;
import com.prolificinteractive.materialcalendarview.DayViewFacade;

import java.util.Collection;
import java.util.HashSet;

public class EventDecorator implements DayViewDecorator {

    private final int color;
    private final HashSet<CalendarDay> dates;
    private TodoFragment.ToDoType toDoType;

    public EventDecorator(Context context, int color, Collection<CalendarDay> dates, TodoFragment.ToDoType toDoType) {
        this.color = context.getResources().getColor(color, null);
        this.dates = new HashSet<>(dates);
        this.toDoType = toDoType;
    }

    @Override
    public boolean shouldDecorate(CalendarDay day) {
        return dates.contains(day);
    }

    @Override
    public void decorate(DayViewFacade view) {

        switch (toDoType) {
            case todo:
                view.addSpan(new CustomDotSpan(5, color, 15));
                break;
            case custom:
                view.addSpan(new CustomDotSpan(5, color, 5));
                break;
            case diary:
                view.addSpan(new CustomDotSpan(5, color, -5));
                break;
            case scan:
                view.addSpan(new CustomDotSpan(5, color, -15));
                break;
        }

    }
}
