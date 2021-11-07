package com.gardify.android.ui.generic;

import android.annotation.SuppressLint;
import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;

import com.android.volley.Request;
import com.gardify.android.R;
import com.gardify.android.databinding.ItemInputHeaderBinding;
import com.gardify.android.ui.generic.interfaces.OnExpandableInputHeaderListener;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.xwray.groupie.ExpandableItem;

import org.jetbrains.annotations.NotNull;
import org.json.JSONArray;
import org.json.JSONException;

import java.util.ArrayList;

public class ExpandableSearchInputHeaderItem extends ExpandableInputHeaderItem implements ExpandableItem {

    public static final int DELAY_3_SECOND = 3000;
    private final Context context;
    private String searchText;
    private final int titleStringResId;
    private boolean hasFocus = false;
    private ArrayAdapter<String> namesAdapter;
    ArrayList<String> stringArrayList = new ArrayList<>();
    private ItemInputHeaderBinding viewBinding;
    private final OnExpandableInputHeaderListener onExpandableInputHeaderListener;

    public ExpandableSearchInputHeaderItem(Context context, int headerTextColor, int backGroundColor, String searchText, @StringRes int titleStringResId, @StringRes int subtitleResId, OnExpandableInputHeaderListener onExpandableInputHeaderListener) {
        super(context, headerTextColor, backGroundColor, titleStringResId, subtitleResId, onExpandableInputHeaderListener);
        this.searchText = searchText;
        this.titleStringResId = titleStringResId;
        this.context = context;
        this.onExpandableInputHeaderListener = onExpandableInputHeaderListener;
    }

    @Override
    public void bind(@NonNull final ItemInputHeaderBinding _binding, int position) {
        super.bind(_binding, position);
        viewBinding = _binding;

        _binding.icon.setVisibility(View.GONE);
        _binding.count.setVisibility(View.INVISIBLE);
        _binding.linearLayoutExpandableHeader.setClickable(false);
        _binding.autoCompleteEditTextName.setEnabled(true);
        if (_binding.autoCompleteEditTextName.isEnabled()) {
            afterTextChangedTriggerOnclick(_binding);
        }
        _binding.plantsNameListview.setEnabled(true);
        _binding.plantsNameListview.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                v.getParent().requestDisallowInterceptTouchEvent(true);
                return false;
            }
        });
    }

    @SuppressLint("ClickableViewAccessibility")
    private void afterTextChangedTriggerOnclick(@NonNull ItemInputHeaderBinding _binding) {

        //Getting the instance of AutoCompleteTextView
        _binding.autoCompleteEditTextName.setThreshold(1);//will start working from first character
        _binding.autoCompleteEditTextName.setText(searchText);


        ListView namesListView = _binding.plantsNameListview;

        namesAdapter = new ArrayAdapter<>(context, R.layout.item_plant_search_suggestion, stringArrayList);
        namesListView.setAdapter(namesAdapter);

        TextWatcher textWatcher = textWatcherListener(stringArrayList, namesListView);

        _binding.autoCompleteEditTextName.setOnFocusChangeListener((v, _hasFocus) -> {
            hasFocus = _hasFocus;
            if (_hasFocus) {
                _binding.autoCompleteEditTextName.addTextChangedListener(textWatcher);
            } else {
                _binding.autoCompleteEditTextName.removeTextChangedListener(textWatcher);
            }
        });

        namesListView.setOnItemClickListener((parent, view, position, id) -> {
            String selectedString = namesAdapter.getItem(position);
            cancelPendingRequests();
            namesListView.setVisibility(View.GONE);
            namesAdapter.clear();
            hasFocus = false;
            namesAdapter.notifyDataSetChanged();
            _binding.autoCompleteEditTextName.clearFocus();
            _binding.autoCompleteEditTextName.setText(selectedString);
            searchText = selectedString;
            notifyChanged();
            onExpandableInputHeaderListener.onClick(titleStringResId, selectedString);
        });

    }

    @NotNull
    private TextWatcher textWatcherListener(ArrayList<String> stringArrayList, ListView listView) {
        return new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {
                RequestQueueSingleton.getInstance(context).cancelAllRequests();
            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
            }

            @Override
            public void afterTextChanged(Editable typedString) {
                if (hasFocus && typedString.length() > 0) {
                    Log.d("ExpandableSearchInputHeaderItem: ", typedString.toString());

                    populateListviewNames(typedString);

                    triggerSearchApi(typedString);
                }
            }

            private void populateListviewNames(Editable typedString) {
                //following line prevents search field being cleared after results are loaded
                searchText = typedString.toString();
                viewBinding.autoCompleteEditTextName.setSelection(viewBinding.autoCompleteEditTextName.getText().toString().length());

                String encodedTypedString = android.net.Uri.encode(String.valueOf(typedString));
                String searchUrl = APP_URL.PLANT_SEARCH_API + "getplantwithname?plantName=" + encodedTypedString;
                RequestQueueSingleton.getInstance(context).arrayRequest(searchUrl, Request.Method.GET, this::onSuccessPlantName, null, null);
            }

            private void onSuccessPlantName(JSONArray jsonArray) {

                namesAdapter.clear();
                stringArrayList.clear();
                for (int i = 0; i < jsonArray.length(); i++) {
                    try {
                        stringArrayList.add(jsonArray.get(i).toString());

                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }
                //namesAdapter.addAll(stringArrayList);
                displayNamesPopupList(stringArrayList, listView);
                namesAdapter.notifyDataSetChanged();
                notifyChanged();
            }
        };
    }

    private void displayNamesPopupList(ArrayList<String> stringArrayList, ListView listView) {
        if (stringArrayList.size() > 0) {
            if (listView.getVisibility() == View.GONE) {
                listView.setVisibility(View.VISIBLE);
            }
        } else {
            listView.setVisibility(View.GONE);
        }
    }

    Handler myHandler = new Handler(Looper.getMainLooper());

    private void cancelPendingRequests() {
        myHandler.removeCallbacksAndMessages(null);
    }

    private void triggerSearchApi(Editable typedString) {
        myHandler.postDelayed(() -> {
            cancelPendingRequests();
            onExpandableInputHeaderListener.onClick(titleStringResId, String.valueOf(typedString));
        }, DELAY_3_SECOND);
    }

    public void resetEditText(String searchText) {
        this.searchText = searchText;
        viewBinding.autoCompleteEditTextName.setSelection(viewBinding.autoCompleteEditTextName.getText().toString().trim().length());
        notifyChanged();
    }

}
