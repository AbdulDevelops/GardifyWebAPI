package com.gardify.android.ui.gardenGlossary.recyclerItem;

import android.content.Context;
import android.content.res.Resources;
import android.text.method.ScrollingMovementMethod;
import android.view.View;

import androidx.annotation.NonNull;

import com.android.volley.Request;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.gardenGlossary.Example;
import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewGardenGlossaryFooterFilterItemBinding;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.xwray.groupie.databinding.BindableItem;

import org.json.JSONException;
import org.json.JSONObject;

public class GardenGlossaryFooterCardItem extends BindableItem<RecyclerViewGardenGlossaryFooterFilterItemBinding> {

    private CharSequence text;
    private Context context;
    private Example example;
    boolean expand = false;
    Resources res;

    public GardenGlossaryFooterCardItem(Context context) {
        this.context=context;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_garden_glossary_footer_filter_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewGardenGlossaryFooterFilterItemBinding viewBinding, int position) {
        res = context.getResources();

        viewBinding.textViewGardenKnowledgeFooterFilterItemHeader.setText(res.getString(R.string.gardenGlossary_themeNotFound));
        viewBinding.textViewGardenKnowledgeFooterFilterItemAsk.setText(res.getString(R.string.gardenGlossary_askOurExperts));
        viewBinding.textViewGardenKnowledgeFooterFilterItemQuestion.setText(res.getString(R.string.plantDoc_question));

        viewBinding.editTextGardenKnowledgeFooter.setMovementMethod(new ScrollingMovementMethod());

        viewBinding.buttonGardenKnowledgeFooterFilterItem.setText(res.getString(R.string.plantDoc_sendQuestion));
        String postUrl = APP_URL.ACCOUNT_API + "contact";
        String subject = "GardeningAZ Thema";
        viewBinding.buttonGardenKnowledgeFooterFilterItem.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                viewBinding.progressbarGardenKnowledgeFooter.setVisibility(View.VISIBLE);
                String textUser = viewBinding.editTextGardenKnowledgeFooter.getText().toString();
                ApplicationUser user = PreferencesUtility.getUser(context);
                // creating json array
                JSONObject obj=new JSONObject();
                try {
                    obj.put("Email", user.getEmail());
                    obj.put("Text",textUser);
                    obj.put("Subject", subject);
                } catch (JSONException e) {
                    e.printStackTrace();
                    viewBinding.progressbarGardenKnowledgeFooter.setVisibility(View.GONE);
                }

                RequestQueueSingleton.getInstance(context).objectRequest(postUrl, Request.Method.POST, this::SendingEmail, null, obj);


            }

            private void SendingEmail(JSONObject jsonObject) {
                //Toast.makeText(context, "Success", Toast.LENGTH_SHORT).show();
                viewBinding.progressbarGardenKnowledgeFooter.setVisibility(View.GONE);
                viewBinding.editTextGardenKnowledgeFooter.setText(context.getResources().getString(R.string.all_hello_comma_blankspace));
            }
        });

    }

    public void setText(CharSequence text) {
        this.text = text;
    }

    public CharSequence getText() {
        return text;
    }

}
