package com.gardify.android.ui.newsletter;

import android.graphics.Typeface;
import android.os.Bundle;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.TextPaint;
import android.text.method.LinkMovementMethod;
import android.text.style.ClickableSpan;
import android.text.style.StyleSpan;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;

import org.json.JSONObject;

import java.util.HashMap;

import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;

public class NewsletterFragment extends Fragment implements View.OnClickListener {

    private static final String TAG = "NewsletterFragment: ";
    private static final int PRIVACY_START_LINK = 72;
    private static final int PRIVACY_END_LINK = 94;
    private static final int TERMS_CONDITION_START_LINK = 105;
    private static final int TERMS_CONDITION_END_LINK = 138;
    private TextView footerTextView;
    private EditText emailEditText;
    private Button newsletterSubscribeBtn, newsletterUnsubscribeBtn;
    private ProgressBar progressBar;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_newsletter, container, false);

        //initialize views
        init(root);
        clickableFooterText();

        setupToolbar(getActivity(), "NEWSLETTER", R.drawable.gardify_icon, R.color.colorPrimary, true);

        return root;
    }

    public void init(View root) {
        /* finding views block */

        footerTextView = root.findViewById(R.id.text_view_newsletter_footer_text);
        newsletterSubscribeBtn = root.findViewById(R.id.button_newsletter_subscribe);
        newsletterUnsubscribeBtn = root.findViewById(R.id.button_newsletter_unsubscribe);
        emailEditText = root.findViewById(R.id.edit_text_newsletter_email);
        progressBar = root.findViewById(R.id.progressBar_newsletter);

        newsletterSubscribeBtn.setOnClickListener(this);
        newsletterUnsubscribeBtn.setOnClickListener(this);
    }

    public void clickableFooterText() {
        SpannableString spannableString = new SpannableString("Hast du Fragen zum Umgang mit Deinen Daten? Informiere dich über unsere Datenschutzgrundsätze und unsere Allgemeinen Geschäftsbedingungen.");

        int privacyStart = PRIVACY_START_LINK, privacyEnd = PRIVACY_END_LINK;
        int termsConditionStart = TERMS_CONDITION_START_LINK, termsConditionEnd = TERMS_CONDITION_END_LINK;

        ClickableSpan clickablePrivacySpan = new ClickableSpan() {
            @Override
            public void onClick(View textView) {
                navigateToFragment(R.id.nav_controller_privacy_policy, getActivity(), true, null);
            }

            @Override
            public void updateDrawState(TextPaint ds) {
                super.updateDrawState(ds);
                ds.setUnderlineText(false);
            }
        };

        ClickableSpan clickableTermsSpan = new ClickableSpan() {
            @Override
            public void onClick(View textView) {
                navigateToFragment(R.id.nav_controller_agb, getActivity(), true, null);

            }

            @Override
            public void updateDrawState(TextPaint ds) {
                super.updateDrawState(ds);
                ds.setUnderlineText(false);
            }
        };

        spannableString.setSpan(clickablePrivacySpan, privacyStart, privacyEnd, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
        spannableString.setSpan(new StyleSpan(Typeface.BOLD), privacyStart, privacyEnd, 0);

        spannableString.setSpan(clickableTermsSpan, termsConditionStart, termsConditionEnd, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
        spannableString.setSpan(new StyleSpan(Typeface.BOLD), termsConditionStart, termsConditionEnd, 0);

        footerTextView.setText(spannableString);
        footerTextView.setMovementMethod(LinkMovementMethod.getInstance());
        footerTextView.setClickable(true);
    }

    @Override
    public void onClick(View v) {

        ApplicationUser user = PreferencesUtility.getUser(getContext());


        switch (v.getId()) {
            case R.id.button_newsletter_subscribe:
                String subscribeLink = APP_URL.NEWSLETTER_API;

                if (validateUserInput()) {
                    HashMap<String, String> hashMap = new HashMap<>();
                    hashMap.put("FirstName", "Platzhalter");
                    hashMap.put("LastName", "Platzhalter");
                    hashMap.put("Email", emailEditText.getText().toString());

                    JSONObject jsonObject = new JSONObject(hashMap);

                    RequestQueueSingleton.getInstance(getContext()).stringRequest(subscribeLink, Request.Method.POST, this::SubscribeSuccess, this::OnError, jsonObject);
                    progressBar.setVisibility(View.VISIBLE);
                }
                break;

            case R.id.button_newsletter_unsubscribe:

                String unsubscribeLink = APP_URL.NEWSLETTER_API +"unsubscribe/" + user.getUserId();
                RequestQueueSingleton.getInstance(getContext()).stringRequest(unsubscribeLink, Request.Method.GET, this::UnsubscribeSuccess, this::OnError, null);
                progressBar.setVisibility(View.VISIBLE);

                break;
        }
    }

    private void OnError(VolleyError error) {
        if (isVisible()) {
            progressBar.setVisibility(View.GONE);
            showErrorDialogNetworkParsed(getContext(), error);
        }
    }

    private void SubscribeSuccess(String jsonObject) {
        if (isVisible()) {
            progressBar.setVisibility(View.GONE);
            Toast.makeText(getContext(), "Newsletter abbestellt" + jsonObject, Toast.LENGTH_SHORT).show();
        }
    }

    private void UnsubscribeSuccess(String jsonObject) {
        if (isVisible()) {
            progressBar.setVisibility(View.GONE);
            Toast.makeText(getContext(), "" + jsonObject, Toast.LENGTH_SHORT).show();
        }
    }

    public Boolean validateUserInput() {

        boolean validFlag = true;

        if (emailEditText.getText().toString().trim().length() == 0) {
            validFlag = false;
            emailEditText.setError(getContext().getResources().getString(R.string.all_required));
        }
        return validFlag;
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