package com.gardify.android.ui.authentication.recyclerItems;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.ui.authentication.interfaces.OnUpdate;
import com.gardify.android.databinding.RecyclerViewAuthGuidedTourBinding;
import com.xwray.groupie.databinding.BindableItem;


/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class AuthGuidedTourRow extends BindableItem<RecyclerViewAuthGuidedTourBinding> {

    private OnUpdate onUpdate;
    private int stringId;

    public AuthGuidedTourRow(int stringId, OnUpdate onUpdate) {
        this.stringId = stringId;
        this.onUpdate = onUpdate;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_auth_guided_tour;
    }

    @Override
    public void bind(@NonNull RecyclerViewAuthGuidedTourBinding binding, int position) {

        binding.textviewAuthGuidedTourDescription.setText("Hier bekommt ihr eine kurze Einführung in die wichtigsten Funktionen.");
        binding.buttonAuthGuidedTourStart.setOnClickListener(v -> {
            //change fragment
            onUpdate.onClick(stringId);
        });
    }
}
