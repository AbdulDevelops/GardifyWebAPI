package com.gardify.android.ui.team;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.gardify.android.R;

import static com.gardify.android.utils.UiUtils.setupToolbar;

public class TeamFragment extends Fragment {

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        setupToolbar(getActivity(), "Teams", R.drawable.gardify_icon, R.color.colorPrimary, true);

        return inflater.inflate(R.layout.fragment_team, container, false);

    }
}