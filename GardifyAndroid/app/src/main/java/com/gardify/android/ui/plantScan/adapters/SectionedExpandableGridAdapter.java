package com.gardify.android.ui.plantScan.adapters;


import android.content.Context;
import android.graphics.Typeface;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.ToggleButton;


import androidx.annotation.NonNull;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;

import com.gardify.android.ui.plantScan.models.ItemImage;

import java.util.ArrayList;

/**
 * Created by lenovo on 2/23/2016.
 */
public class SectionedExpandableGridAdapter extends RecyclerView.Adapter {

    //data array
    private ArrayList<Object> mDataArrayList;

    //context
    private final Context mContext;

    //listeners
    private final ItemClickListener mItemClickListener;
    private final SectionStateChangeListener mSectionStateChangeListener;

    //view type
    private static final int VIEW_TYPE_SECTION = R.layout.layout_section;
    private static final int VIEW_TYPE_ITEM = R.layout.layout_item;

    public SectionedExpandableGridAdapter(Context context, ArrayList<Object> dataArrayList,
                                          final GridLayoutManager gridLayoutManager, ItemClickListener itemClickListener,
                                          SectionStateChangeListener sectionStateChangeListener) {
        mContext = context;
        mItemClickListener = itemClickListener;
        mSectionStateChangeListener = sectionStateChangeListener;
        mDataArrayList = dataArrayList;

        gridLayoutManager.setSpanSizeLookup(new GridLayoutManager.SpanSizeLookup() {
            @Override
            public int getSpanSize(int position) {
                return isSection(position) ? gridLayoutManager.getSpanCount() : 1;
            }
        });
    }

    private boolean isSection(int position) {
        return mDataArrayList.get(position) instanceof Section;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        return new ViewHolder(LayoutInflater.from(mContext).inflate(viewType, parent, false), viewType);
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof ViewHolder) {
            // setheadersdata_flag = true;
            ViewHolder holder1 = (ViewHolder) holder;
            switch (holder.getItemViewType()) {
                case VIEW_TYPE_ITEM:
                    //final Item item = (Item) mDataArrayList.get(position);
                    if (mDataArrayList.get(position) instanceof ItemImage) {
                        final ItemImage itemImage = (ItemImage) mDataArrayList.get(position);
                        //holder.itemTextView.setText(item.getName());
                        holder1.itemImageView.setImageBitmap(itemImage.getImageView());
                        holder1.itemImageView.setImageURI(itemImage.getImageViewUri());
                        holder1.view.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                mItemClickListener.itemClicked(itemImage);
                            }
                        });
                        holder1.itemTextView.setVisibility(View.GONE);
                    } else {
                        final String textItem = (String) mDataArrayList.get(position);
                        if (textItem != null) {
                            if(position==1){
                                holder1.itemTextView.setTypeface(null, Typeface.BOLD);
                            }
                            holder1.view.setOnClickListener(new View.OnClickListener() {
                                @Override
                                public void onClick(View v) {
                                    mItemClickListener.itemClicked(textItem);
                                }
                            });
                            holder1.itemImageView.setVisibility(View.GONE);
                            holder1.itemTextView.setVisibility(View.VISIBLE);
                            holder1.itemTextView.setText(textItem);
                        }
                    }
                    break;
                case VIEW_TYPE_SECTION:
                    final Section section = (Section) mDataArrayList.get(position);
                    holder1.sectionTextView.setText(section.getName());

                    holder1.sectionTextView.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View v) {
                            mItemClickListener.itemClicked(section);
                        }
                    });
                    //holder.sectionToggleButton.setChecked(section.isExpanded);
                    holder1.sectionToggleButton.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
                        @Override
                        public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                            mSectionStateChangeListener.onSectionStateChanged(section, isChecked);
                            if (isChecked) {
                                holder1.sectionToggleButton.setBackgroundDrawable(mContext.getResources().getDrawable(R.drawable.collapse,null));
                            } else {
                                holder1.sectionToggleButton.setBackgroundDrawable(mContext.getResources().getDrawable(R.drawable.expand,null));
                            }
                        }
                    });
                    break;
            }
        }
    }


    @Override
    public int getItemCount() {
        return mDataArrayList.size();
    }

    @Override
    public int getItemViewType(int position) {
        if (isSection(position))
            return VIEW_TYPE_SECTION;
        else return VIEW_TYPE_ITEM;
    }

    protected class ViewHolder extends RecyclerView.ViewHolder {

        //common
        View view;
        int viewType;

        //for section
        TextView sectionTextView;
        ToggleButton sectionToggleButton;

        //for item
        ImageView itemImageView;
        TextView itemTextView;

        public ViewHolder(View view, int viewType) {
            super(view);
            this.viewType = viewType;
            this.view = view;
            if (viewType == VIEW_TYPE_ITEM) {
                //itemTextView = (TextView) view.findViewById(R.id.text_item);
                itemImageView = view.findViewById(R.id.image_item);
                itemTextView = view.findViewById(R.id.text_view_plant_scan_section_adapter_item);
            } else {
                sectionTextView = (TextView) view.findViewById(R.id.text_section);
                sectionToggleButton = (ToggleButton) view.findViewById(R.id.toggle_button_section);
            }
        }
    }
}

