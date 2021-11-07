package com.gardify.android.ui.generic.recyclerItem;

import android.annotation.SuppressLint;
import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import androidx.annotation.NonNull;
import androidx.annotation.StringRes;
import androidx.core.content.res.ResourcesCompat;

import com.android.volley.Request;
import com.gardify.android.R;
import com.gardify.android.databinding.ItemGenericInputSearchBinding;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.xwray.groupie.databinding.BindableItem;

import org.jetbrains.annotations.NotNull;
import org.json.JSONArray;
import org.json.JSONException;

import java.util.ArrayList;

public class GenericInputSearch extends BindableItem<ItemGenericInputSearchBinding> {

    public static final int DELAY_3_SECOND = 3000;
    //builder
    private final Context context;
    private String searchText;
    private final int titleStringResId;
    private final OnSearchInputListener onSearchInputListener;
    private int iconStringResId = 0;
    private int backgroundResId = 0;

    //search field related
    private boolean hasFocus = false;
    private ArrayAdapter<String> namesAdapter;
    private ArrayList<String> stringArrayList = new ArrayList<>();
    //binding
    private ItemGenericInputSearchBinding viewBinding;

    public GenericInputSearch(Builder builder) {
        this.context = builder.context;
        this.titleStringResId = builder.titleStringResId;
        this.iconStringResId = builder.iconStringResId;
        this.backgroundResId = builder.backgroundResId;
        this.onSearchInputListener = builder.onSearchInputListener;
    }


    @Override
    public int getLayout() {
        return R.layout.item_generic_input_search;
    }

    @Override
    public void bind(@NonNull final ItemGenericInputSearchBinding _binding, int position) {
        viewBinding = _binding;

        setIcon(_binding);
        setBackground(_binding);
        setTitle(_binding);

        afterTextChangedTriggerOnclick(_binding);
        enablePlantListViewScroll(_binding);
    }

    private void setIcon(ItemGenericInputSearchBinding _binding) {
        if (iconStringResId != 0) {
            _binding.imageViewSearchInputIcon.setVisibility(View.VISIBLE);
            _binding.imageViewSearchInputIcon.setImageResource(iconStringResId);
        } else {
            _binding.imageViewSearchInputIcon.setVisibility(View.GONE);
        }
    }

    private void setBackground(ItemGenericInputSearchBinding _binding) {
        if (backgroundResId != 0) {
            _binding.autoCompleteEditTextName.setBackground(ResourcesCompat.getDrawable(context.getResources(), backgroundResId, null));
            int leftPaddingPx = 55;
            _binding.autoCompleteEditTextName.setPadding(leftPaddingPx, 0, 0, 0);

        } else {
            _binding.autoCompleteEditTextName.setBackground(null);
            _binding.autoCompleteEditTextName.setPadding(0, 0, 0, 0);

        }
    }

    private void setTitle(ItemGenericInputSearchBinding _binding) {
        if (titleStringResId != 0) {
            _binding.title.setVisibility(View.VISIBLE);
            _binding.title.setText(context.getResources().getString(titleStringResId));
        } else {
            _binding.title.setVisibility(View.GONE);
        }
    }

    @SuppressLint("ClickableViewAccessibility")
    private void enablePlantListViewScroll(@NonNull com.gardify.android.databinding.ItemGenericInputSearchBinding _binding) {
        _binding.plantsNameListview.setOnTouchListener((v, event) -> {
            v.getParent().requestDisallowInterceptTouchEvent(true);
            return false;
        });
    }

    @SuppressLint("ClickableViewAccessibility")
    private void afterTextChangedTriggerOnclick(@NonNull ItemGenericInputSearchBinding _binding) {

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
            onSearchInputListener.onClick(titleStringResId, selectedString);
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
                viewBinding.autoCompleteEditTextName.setSelection(viewBinding.autoCompleteEditTextName.getText().toString().trim().length());

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
            onSearchInputListener.onClick(titleStringResId, String.valueOf(typedString));
        }, DELAY_3_SECOND);
    }

    public void resetEditText(String searchText) {
        this.searchText = searchText;
        viewBinding.autoCompleteEditTextName.setSelection(viewBinding.autoCompleteEditTextName.getText().toString().trim().length());
        notifyChanged();
    }

    public static class Builder {
        private final Context context;
        private int titleStringResId;
        private int iconStringResId;
        private int backgroundResId;

        private OnSearchInputListener onSearchInputListener;

        public Builder(Context context) {
            this.context = context;
        }

        public Builder setTitle(int titleStringResId) {
            this.titleStringResId = titleStringResId;
            return this;
        }

        public Builder setIcon(int iconStringResId) {
            this.iconStringResId = iconStringResId;
            return this;
        }

        public Builder setBackground(int backgroundResId) {
            this.backgroundResId = backgroundResId;
            return this;
        }

        public Builder setImageClickListener(OnSearchInputListener onSearchInputListener) {
            this.onSearchInputListener = onSearchInputListener;
            return this;
        }

        public GenericInputSearch build() {
            return new GenericInputSearch(this);
        }

    }

    public interface OnSearchInputListener {
        void onClick(@StringRes int titleStringResId, String typedText);
    }

}
