package com.gardify.android.ui.saveToGarden;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CheckBox;
import android.widget.TextView;

import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.data.saveToGarden.PlantList;
import com.gardify.android.R;

import java.util.List;

public class SaveToGardenRecyclerAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

    private List<PlantList> plantsList;
    private Context context;
    private OnItemClickListener onItemClickListener;

    // handling checkbox selection
    private boolean isMultiSelection;
    private int selectedPosition = -1;// no selection by default


    public SaveToGardenRecyclerAdapter(Context context, List<PlantList> plantsList, boolean isMultiSelection, OnItemClickListener onClickListener) {
        this.context = context;
        this.plantsList = plantsList;
        this.isMultiSelection = isMultiSelection;
        onItemClickListener = onClickListener;
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View itemView = LayoutInflater.from(parent.getContext()).inflate(R.layout.recycler_view_save_to_garden_row_item, parent, false);
        return new ItemViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(final RecyclerView.ViewHolder holder, int position) {
        /*
        position 0 is for header
        */
        if (holder instanceof ItemViewHolder) {

            final ItemViewHolder itemViewHolder = (ItemViewHolder) holder;

            itemViewHolder.onBind(position, itemViewHolder);

        }
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public int getItemCount() {
        return plantsList.size();
    }

    public class ItemViewHolder extends RecyclerView.ViewHolder {

        private TextView textViewName;
        private CheckBox checkBox;

        public ItemViewHolder(View itemView) {
            super(itemView);

            textViewName = itemView.findViewById(R.id.textView_save_to_garden_name);
            checkBox = itemView.findViewById(R.id.checkBox_save_to_garden);
        }

        private void onBind(int position, ItemViewHolder holder) {
            holder.textViewName.setText(plantsList.get(position).getName());
            holder.checkBox.setOnClickListener(v -> {
                if(!isMultiSelection){
                    selectedPosition=position;
                    notifyDataSetChanged();
                }
                onItemClickListener.OnItemClickListener(holder.checkBox, position);
            });

            if (!isMultiSelection) {

                if (selectedPosition == position) {
                    plantsList.get(position).setListSelected(true);
                } else {
                    plantsList.get(position).setListSelected(false);
                }
            }

            holder.checkBox.setChecked(plantsList.get(position).getListSelected());

        }
    }

    public interface OnItemClickListener {
        void OnItemClickListener(View view, int position);
    }

    public void Update(List<PlantList> plantLists) {
        this.plantsList = plantLists;
        notifyDataSetChanged();
    }


}


