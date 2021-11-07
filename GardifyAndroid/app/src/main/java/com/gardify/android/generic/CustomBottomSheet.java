package com.gardify.android.generic;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import com.gardify.android.R;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;

public class CustomBottomSheet<T> extends BottomSheetDialogFragment {

    private Button button;
    private TextView headerTextView;
    private OnBottomSheetClickListener onBottomSheetClickListener;
    private String header, buttonName;
    private int stringId;
    private T object;
    public CustomBottomSheet(int stringId, T object, String header, String buttonName, OnBottomSheetClickListener onBottomSheetClickListener) {
        this.header=header;
        this.object=object;
        this.stringId=stringId;
        this.buttonName=buttonName;
        this.onBottomSheetClickListener = onBottomSheetClickListener;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {

        View view = inflater.inflate(R.layout.bottom_sheet_dialog,container,false);

        button = view.findViewById(R.id.bottom_nav_button);
        headerTextView = view.findViewById(R.id.bottom_nav_header);

        headerTextView.setText(header);
        button.setText(buttonName);

        button.setOnClickListener(v -> {
            onBottomSheetClickListener.onClick(stringId, object);
            dismiss();
        });


        return view;
    }

    public interface OnBottomSheetClickListener<T> {
        void onClick(int stringId, T object);
    }
}