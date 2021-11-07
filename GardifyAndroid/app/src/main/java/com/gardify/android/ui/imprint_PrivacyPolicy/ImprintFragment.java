package com.gardify.android.ui.imprint_PrivacyPolicy;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import com.gardify.android.R;

import static com.gardify.android.utils.UiUtils.setupToolbar;

public class ImprintFragment extends Fragment {


    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_imprint, container, false);

        setupToolbar(getActivity(), "IMPRESSUM", R.drawable.gardify_icon, R.color.colorPrimary,true);

        return root;
    }

    public void init(View root) {
        /* finding views block */
    }

}