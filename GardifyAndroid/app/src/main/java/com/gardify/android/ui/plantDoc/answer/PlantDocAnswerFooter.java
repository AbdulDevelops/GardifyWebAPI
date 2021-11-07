package com.gardify.android.ui.plantDoc.answer;

import android.app.Activity;
import android.content.Context;
import android.util.AttributeSet;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;

import com.android.volley.Request;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.plantsDocModel.PlanDocViewModelAnswer;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.generic.RecycleViewGenericAdapter;
import com.gardify.android.utils.RequestQueueSingleton;

import org.json.JSONObject;

import java.util.HashMap;

import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class PlantDocAnswerFooter extends LinearLayout implements RecycleViewGenericAdapter.RecyclerViewRowFooter<PlanDocViewModelAnswer>{

    Button buttonAnswer;
    TextView textViewAnswer;
    EditText editTextAnswer;
    Button buttonSendAnswer;



    public PlantDocAnswerFooter(Context context) {
        super(context);
    }

    public PlantDocAnswerFooter(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public PlantDocAnswerFooter(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();

        buttonAnswer = findViewById(R.id.button_plant_doc_answer_footer_answer);
        textViewAnswer = findViewById(R.id.textView_plant_doc_answer_footer_answer);
        editTextAnswer = findViewById(R.id.button_plant_doc_answer_footer_answer_text);
        buttonSendAnswer = findViewById(R.id.button_plant_doc_answer_footer_send_answer);
    }


    @Override
    public void showData(PlanDocViewModelAnswer item) {
        buttonAnswer.setOnClickListener(v -> {
            textViewAnswer.setVisibility(VISIBLE);
            editTextAnswer.setVisibility(VISIBLE);
            buttonSendAnswer.setVisibility(VISIBLE);
        });

        buttonSendAnswer.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                try {
                    ApplicationUser user = PreferencesUtility.getUser(getContext());
                    String text = editTextAnswer.getText().toString();
                    HashMap<String, String> hashMap = new HashMap<>();
                    hashMap.put("AnswerText", text);
                    hashMap.put("AuthorId", user.getUserId());
                    hashMap.put("PlantDocEntryId", String.valueOf(item.getPlantDocViewModel().getQuestionId()));

                    JSONObject jsonObject = new JSONObject(hashMap);
                    String apiUrl = APP_URL.PLANT_DOC_ROUTE + "answer";
                    RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUrl, Request.Method.POST, this::send, null, jsonObject);


                } catch (Exception e) {
                    Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();

                }
            }

            private void send(String jsonObject) {
                if (jsonObject != null) {
                    Toast.makeText(getContext(), "Eintrag wurde erstellt", Toast.LENGTH_SHORT).show();
                    navigateToFragment(R.id.nav_controller_plant_doc, (Activity) getContext(), false, null);

                }
            }
        });
    }
}
