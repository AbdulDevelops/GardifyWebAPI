package com.gardify.android.ui.plantDoc;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.util.AttributeSet;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.Nullable;

import com.gardify.android.data.plantsDocModel.PlantDocViewModel;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.generic.RecycleViewGenericAdapter;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;
import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class PlantDocRow extends LinearLayout implements RecycleViewGenericAdapter.RecyclerViewRow<PlantDocViewModel> {


    private ImageView imageViewPlant;
    private TextView textViewDate;
    private TextView textViewHeader;
    private TextView textViewAnswer;
    private TextView textViewQuestion;
    private TextView textViewQuestionHeader;
    private ImageView imageViewButton;
    boolean expand = true;


    public PlantDocRow(Context context) {
        super(context);
    }

    public PlantDocRow(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public PlantDocRow(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();

        imageViewPlant = findViewById(R.id.imageView_plant_doc_plant_item_plant_image);
        textViewDate = findViewById(R.id.textView_plant_doc_row_item_date);
        textViewHeader = findViewById(R.id.textView_plant_doc_row_item_header);
        textViewAnswer = findViewById(R.id.textView_plant_doc_row_item_answer);
        textViewQuestion = findViewById(R.id.textView_plant_doc_row_item_question);
        textViewQuestionHeader = findViewById(R.id.textView_view_plant_doc_row_question_header);
        imageViewButton = findViewById(R.id.imageView_plant_doc_row_item_button);
    }

    @Override
    public void showData(PlantDocViewModel item) {

        SimpleDateFormat inFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
        SimpleDateFormat outFormat = new SimpleDateFormat("dd.MM.yyyy");
        Date d = null;

        String imageUrl = APP_URL.BASE_ROUTE + item.getImages().get(0).getSrcAttr();
        loadImageUsingGlide(getContext(),imageUrl,imageViewPlant);

        try {
            d = inFormat.parse(item.getPublishDate());
        } catch (ParseException e) {
            e.printStackTrace();
        }

        String formattedDate = outFormat.format(d);

        textViewDate.setText(formattedDate);
        textViewHeader.setText(item.getThema());
        if (item.getTotalAnswers() < 2) {
            textViewAnswer.setText(item.getTotalAnswers() + " Antwort");
        } else {
            textViewAnswer.setText(item.getTotalAnswers() + " Antworten");
        }

        imageViewButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {

                Bundle args = new Bundle();
                args.putString("questionId", item.getQuestionId().toString());
                navigateToFragment(R.id.nav_controller_plant_doc_answer, (Activity) getContext(), false, args);

            }
        });

    }
}
