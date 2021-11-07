package com.gardify.android.ui.todosCalendar.calendarDecorator;

import android.graphics.Canvas;
import android.graphics.Paint;
import android.text.style.LineBackgroundSpan;

public class CustomDotSpan implements LineBackgroundSpan {


    private final float radius;
    private int color;
    private int dotPosition;

    public CustomDotSpan(float radius, int color, int dotPosition) {
        this.radius = radius;
        this.color = color;
        this.dotPosition = dotPosition;
    }

    @Override
    public void drawBackground(Canvas canvas, Paint paint, int left, int right, int top, int baseline,
                               int bottom, CharSequence text, int start, int end, int lnum) {

        int oldColor = paint.getColor();
        if (color != 0) {
            paint.setColor(color);
        }
        float xPosition = (left + right) / 2 - dotPosition;
        canvas.drawCircle(xPosition, bottom + radius, radius, paint);
        paint.setColor(oldColor);

    }
}