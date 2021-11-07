package com.gardify.android.ui.myGarden.recyclerItems;

import android.content.Context;
import android.graphics.Typeface;
import android.view.View;

import androidx.annotation.ColorInt;
import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.data.myGarden.UserGarden.UserGarden;
import com.gardify.android.databinding.RecyclerViewMyGardenUserGardenListenItemBinding;
import com.xwray.groupie.databinding.BindableItem;

/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class ListenCardItem extends BindableItem<RecyclerViewMyGardenUserGardenListenItemBinding> {

    private UserGarden userGarden;
    private int nameStringId;
    private OnUserListClickListener onUserListClickListener;
    private Context context;
    private int selectedUserGardenListId;
    @ColorInt
    private int colorRes;

    public ListenCardItem(@ColorInt int colorRes, Context context, int selectedUserGardenListId, UserGarden userGarden, OnUserListClickListener onUserListClickListener) {
        this.userGarden = userGarden;
        this.onUserListClickListener = onUserListClickListener;
        this.context = context;
        this.selectedUserGardenListId = selectedUserGardenListId;
        this.colorRes = colorRes;
    }

    public ListenCardItem(@ColorInt int colorRes, Context context, int selectedUserGardenListId, int nameStringId, OnUserListClickListener onUserListClickListener) {
        this.nameStringId = nameStringId;
        this.onUserListClickListener = onUserListClickListener;
        this.selectedUserGardenListId = selectedUserGardenListId;
        this.context = context;
        this.colorRes = colorRes;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_my_garden_user_garden_listen_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewMyGardenUserGardenListenItemBinding viewBinding, int position) {
        if (userGarden != null) {

            if (selectedUserGardenListId == userGarden.getId()) {
                viewBinding.textViewMyGardenListenName.setTypeface(Typeface.DEFAULT_BOLD);
            } else {
                viewBinding.textViewMyGardenListenName.setTypeface(Typeface.DEFAULT);
            }

            viewBinding.textViewMyGardenListenName.setText(userGarden.getName());
        } else {
            viewBinding.textViewMyGardenListenName.setText(context.getResources().getString(nameStringId));
            viewBinding.imageViewMyGardenListenDelete.setVisibility(View.INVISIBLE);
            viewBinding.imageViewMyGardenMoreOptions.setVisibility(View.INVISIBLE);
        }

        viewBinding.linearLayout.setBackgroundColor(colorRes);

        // Click listeners
        viewBinding.textViewMyGardenListenName.setOnClickListener(v -> {
            onUserListClickListener.onClick(userGarden, viewBinding, viewBinding.textViewMyGardenListenName, position);
        });

        viewBinding.imageViewMyGardenListenDelete.setOnClickListener(v -> {
            onUserListClickListener.onClick(userGarden, viewBinding, viewBinding.imageViewMyGardenListenDelete, position);
        });

        viewBinding.imageViewMyGardenMoreOptions.setOnClickListener(v -> {
            onUserListClickListener.onClick(userGarden, viewBinding, viewBinding.imageViewMyGardenMoreOptions, position);
        });
    }

    public interface OnUserListClickListener {
        void onClick(UserGarden userGarden, RecyclerViewMyGardenUserGardenListenItemBinding viewBinding, View view, int Pos);
    }
}
