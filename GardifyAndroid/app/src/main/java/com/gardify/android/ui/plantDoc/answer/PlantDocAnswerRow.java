package com.gardify.android.ui.plantDoc.answer;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.Nullable;
import androidx.cardview.widget.CardView;

import com.gardify.android.data.plantsDocModel.PlantDocAnswerList;
import com.gardify.android.R;
import com.gardify.android.generic.RecycleViewGenericAdapter;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

public class PlantDocAnswerRow extends LinearLayout implements RecycleViewGenericAdapter.RecyclerViewRow<PlantDocAnswerList> {

    private TextView textViewDate;
    private TextView textViewAuthor;
    private TextView textViewAnswer;
    private ImageView imageView;
    private CardView cardView;

    public PlantDocAnswerRow(Context context) {
        super(context);
    }

    public PlantDocAnswerRow(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public PlantDocAnswerRow(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();

        textViewAnswer = findViewById(R.id.textView_plant_doc_answer_row_item_answer123);
        textViewDate = findViewById(R.id.textView_plant_doc_answer_row_item_date);
        textViewAuthor = findViewById(R.id.textView_plant_doc_answer_row_item_author);
        imageView = findViewById(R.id.imageView_plant_doc_answer_row_item);
        cardView = findViewById(R.id.card_view_plant_doc_answer_row_item);
    }

    @Override
    public void showData(PlantDocAnswerList item) {

        textViewAuthor.setText("Antwort von " +item.getAutorName());

        SimpleDateFormat inFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
        SimpleDateFormat outFormat = new SimpleDateFormat("dd.MM.yyyy");
        Date d = null;
        try {
            d = inFormat.parse(item.getDate());
        } catch (ParseException e) {
            e.printStackTrace();
        }

        String formattedDate = outFormat.format(d);

        textViewDate.setText(formattedDate);
        imageView.setVisibility(GONE);

        if (item.getAutorName().equals("mp")) {
            cardView.setCardBackgroundColor(getContext().getResources().getColor(R.color.colorPrimary));
            textViewAnswer.setTextColor(getContext().getResources().getColor(R.color.text_all_white));
            textViewAuthor.setTextColor(getContext().getResources().getColor(R.color.text_all_white));
            textViewAuthor.setPadding(30,0,0,0);
            textViewDate.setTextColor(getContext().getResources().getColor(R.color.text_all_white));
            imageView.setVisibility(VISIBLE);
        }
        textViewAnswer.setText(item.getAnswerText());


    }
}
