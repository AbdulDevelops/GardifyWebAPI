package com.gardify.android.ui.todosCalendar.calendarDecorator;

import android.content.Context;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;

import com.gardify.android.R;
import com.prolificinteractive.materialcalendarview.CalendarDay;
import com.prolificinteractive.materialcalendarview.DayViewDecorator;
import com.prolificinteractive.materialcalendarview.DayViewFacade;

/**
 * Highlight weekdays with a background
 */
public class WeekDayBackgroundDecorator implements DayViewDecorator {

    private final Drawable highlightDrawable;
    private static int color;

    public WeekDayBackgroundDecorator(Context context) {
        highlightDrawable = new ColorDrawable(color);
        color = context.getResources().getColor(R.color.greenTransparent,null);
    }

    @Override public boolean shouldDecorate(final CalendarDay day) {
        return true; // applies for all days in week
    }

    @Override public void decorate(final DayViewFacade view) {
        view.setBackgroundDrawable(highlightDrawable);
    }
}