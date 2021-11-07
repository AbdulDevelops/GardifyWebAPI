package com.gardify.android.ui.plantDoc.myPostAnswer;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.Nullable;
import androidx.cardview.widget.CardView;

import com.gardify.android.data.plantsDocModel.PlantDocAnswerList;
import com.gardify.android.R;
import com.gardify.android.generic.RecycleViewGenericAdapter;

public class PlantDocMyPostAnswerRow extends LinearLayout implements RecycleViewGenericAdapter.RecyclerViewRow<PlantDocAnswerList>{
    private TextView textViewAnswer;
    private CardView cardView;
    public PlantDocMyPostAnswerRow(Context context) {
        super(context);
    }

    public PlantDocMyPostAnswerRow(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public PlantDocMyPostAnswerRow(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        textViewAnswer = findViewById(R.id.textView_plant_doc_my_post_answer_row_item_answer);
        cardView = findViewById(R.id.card_view_plant_doc_my_post_answer_row_item);
    }

    @Override
    public void showData(PlantDocAnswerList item) {
            if(item.getAutorName().equals("mp")){
                cardView.setBackgroundColor(getContext().getResources().getColor(R.color.colorPrimary));
                textViewAnswer.setTextColor(getContext().getResources().getColor(R.color.text_all_white));
            }
            textViewAnswer.setText(item.getAnswerText());

    }

}
