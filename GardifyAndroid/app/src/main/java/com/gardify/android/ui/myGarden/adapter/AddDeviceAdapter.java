package com.gardify.android.ui.myGarden.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CheckBox;
import android.widget.TextView;

import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.data.myGarden.AdminDevice.AdminDevice;
import com.gardify.android.R;

import java.util.List;

public class AddDeviceAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

    private static final int TYPE_HEADER = 0;
    private static final int TYPE_ITEM = 1;
    private OnItemClickListener onItemClickListener;
    private List<AdminDevice> adminDeviceList;
    private String tagId;
    private Context context;

    public AddDeviceAdapter(Context context, List<AdminDevice> adminDeviceList, OnItemClickListener onClickListener) {
        this.context = context;
        this.onItemClickListener = onClickListener;
        this.adminDeviceList = adminDeviceList;

    }
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        if (viewType == TYPE_ITEM) {
            // Here Inflating your recyclerview item layout
            View itemView = LayoutInflater.from(parent.getContext()).inflate(R.layout.fragment_filter_item_row, parent, false);
            return new ItemViewHolder(itemView, onItemClickListener);
        } else if (viewType == TYPE_HEADER) {
            // Here Inflating your header view
            View itemView = LayoutInflater.from(parent.getContext()).inflate(R.layout.fragment_filter_item_row_header, parent, false);
            return new HeaderViewHolder(itemView, onItemClickListener);
        } else return null;
    }

    @Override
    public void onBindViewHolder(final RecyclerView.ViewHolder holder, int position) {

        if (holder instanceof HeaderViewHolder) {
            // setheadersdata_flag = true;
            HeaderViewHolder headerViewHolder = (HeaderViewHolder) holder;
            // You have to set your header items values with the help of model class and you can modify as per your needs


        } else if (holder instanceof ItemViewHolder) {

            final ItemViewHolder itemViewHolder = (ItemViewHolder) holder;

                // for name filter fragment
                itemViewHolder.textViewName.setText(adminDeviceList.get(position - 1).getName());
                itemViewHolder.checkBoxTags.setVisibility(View.VISIBLE);
                itemViewHolder.checkBoxTags.setChecked(adminDeviceList.get(position - 1).isCheckedFlag());

        }
    }

    @Override
    public int getItemViewType(int position) {
        if (position == 0) {
            return TYPE_HEADER;
        }
        return TYPE_ITEM;
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public int getItemCount() {
        return adminDeviceList.size() + 1;

    }


    public interface OnItemClickListener {
        void OnItemClickListener(View view, int position);
    }

    private class HeaderViewHolder extends RecyclerView.ViewHolder implements View.OnClickListener {


        public HeaderViewHolder(View headerView, OnItemClickListener onItemClickListener) {
            super(headerView);
        }

        @Override
        public void onClick(View view) {
        }

    }

    public class ItemViewHolder extends RecyclerView.ViewHolder implements View.OnClickListener {

        TextView textViewName;
        CheckBox checkBoxTags;

        public ItemViewHolder(View itemView, OnItemClickListener onItemClickListener) {
            super(itemView);

            textViewName = itemView.findViewById(R.id.textView_filter_item_row_category_name);
            checkBoxTags = itemView.findViewById(R.id.checkbox_filter_item_row_plant_search_filter);

            textViewName.setOnClickListener(this);
            checkBoxTags.setOnClickListener(this);
        }

        @Override
        public void onClick(View view) {
            onItemClickListener.OnItemClickListener(view, getAdapterPosition() - 1);

        }
    }
}


