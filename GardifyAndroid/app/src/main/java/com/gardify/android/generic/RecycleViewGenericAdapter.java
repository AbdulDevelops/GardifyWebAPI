package com.gardify.android.generic;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.List;

/**
 * @param <T> is generic parameter type provided to List, OnRecyclerViewItemClickListener and RecyclerViewRow
 * @param <V> generic type for header view Model
 * @param <E> generic type for footer view Model
 */
public class RecycleViewGenericAdapter<T, V, E> extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private List<T> mDataset;
    private V mHeaderData;
    private E mFooterData;

    private static final int TYPE_HEADER = 000;
    private static final int TYPE_ITEM = 111;
    private static final int TYPE_FOOTER = 222;

    private OnItemClickListener<T> onItemClickListener;
    private OnItemClickListenerHeader<V> onItemClickListenerHeader;
    private OnItemClickListenerFooter<E> onItemClickListenerFooter;
    private OnTextChangeListener onTextChangeListener;

    private int layoutId, layoutIdHeader, layoutIdFooter;

    public RecycleViewGenericAdapter(List<T> mDataset, int layoutId, V mHeaderData, int layoutIdHeader,
                                     E mFooterData, int layoutIdFooter, OnItemClickListener<T> onItemClickListener,
                                     OnItemClickListenerHeader onItemClickListenerHeader, OnItemClickListenerFooter onItemClickListenerFooter) {
        this.onItemClickListener = onItemClickListener;
        this.onItemClickListenerHeader = onItemClickListenerHeader;
        this.onItemClickListenerFooter = onItemClickListenerFooter;
        this.mDataset = mDataset;
        this.mHeaderData = mHeaderData;
        this.mFooterData = mFooterData;
        this.layoutId = layoutId;
        this.layoutIdHeader = layoutIdHeader;
        this.layoutIdFooter = layoutIdFooter;
    }

    public RecycleViewGenericAdapter(List<T> mDataset, int layoutId, V mHeaderData, int layoutIdHeader,
                                     E mFooterData, int layoutIdFooter, OnItemClickListener<T> onItemClickListener,
                                     OnItemClickListenerHeader onItemClickListenerHeader, OnItemClickListenerFooter onItemClickListenerFooter,
                                     OnTextChangeListener onTextChangeListener) {
        this.onItemClickListener = onItemClickListener;
        this.onItemClickListenerHeader = onItemClickListenerHeader;
        this.onItemClickListenerFooter = onItemClickListenerFooter;
        this.mDataset = mDataset;
        this.mHeaderData = mHeaderData;
        this.mFooterData = mFooterData;
        this.layoutId = layoutId;
        this.layoutIdHeader = layoutIdHeader;
        this.layoutIdFooter = layoutIdFooter;
        this.onTextChangeListener= onTextChangeListener;
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        RecyclerView.ViewHolder vh = null;
        if (viewType == TYPE_ITEM) {
            RecyclerViewRow<T> row = (RecyclerViewRow<T>) LayoutInflater.from(parent.getContext()).inflate(layoutId, parent, false);
            vh = new ItemViewHolder(row);
        }else if (viewType == TYPE_HEADER) {
            RecyclerViewRowHeader<V> rowHeader = (RecyclerViewRowHeader<V>) LayoutInflater.from(parent.getContext()).inflate(layoutIdHeader, parent, false);
            vh = new HeaderViewHolder(rowHeader);
        } else if(viewType == TYPE_FOOTER) {
            RecyclerViewRowFooter<E> footerRow = (RecyclerViewRowFooter<E>) LayoutInflater.from(parent.getContext()).inflate(layoutIdFooter, parent, false);
            vh = new FooterViewHolder(footerRow);
        }
        return vh;
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, final int position) {
        if (holder instanceof RecycleViewGenericAdapter.ItemViewHolder) {
            ItemViewHolder itemViewHolder = (ItemViewHolder) holder;
            itemViewHolder.onBind((mHeaderData!=null? position-1 : position), itemViewHolder);
        } else if (holder instanceof RecycleViewGenericAdapter.HeaderViewHolder) {
            HeaderViewHolder headerViewHolder = (HeaderViewHolder) holder;
            headerViewHolder.onBind(headerViewHolder);
        } else if (holder instanceof RecycleViewGenericAdapter.FooterViewHolder) {
            FooterViewHolder footerViewHolder = (FooterViewHolder) holder;
            footerViewHolder.onBind(footerViewHolder);
        }
    }

    @Override
    public int getItemCount() {
        if (mHeaderData != null && mFooterData != null) {
            return mDataset.size() + 2;
        } else if (mHeaderData != null || mFooterData != null) {
            return mDataset.size() + 1;
        }
        return mDataset.size();
    }

    private int getLastPosition() {
        return getItemCount() - 1;
    }

    private boolean isLastPosition(int position) {
        return position == getLastPosition();
    }

    @Override
    public int getItemViewType(int position) {
        if (position == 0 && mHeaderData!=null) {
            return TYPE_HEADER;
        } else if (isLastPosition(position) && mFooterData!=null) {
            return TYPE_FOOTER;
        } else {
            return TYPE_ITEM;
        }
    }

    /**
     * Item ViewHolder
     **/
    public class ItemViewHolder extends RecyclerView.ViewHolder {
        public RecyclerViewRow<T> mRow;

        public ItemViewHolder(RecyclerViewRow<T> itemView) {
            super((View) itemView);
            mRow = itemView;
        }

        private void onBind(final int position, ItemViewHolder viewHolder) {

            viewHolder.mRow.showData(mDataset.get(position));
            ((View) viewHolder.mRow).setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    if (onItemClickListener != null) {
                        onItemClickListener.onItemClick(mDataset.get(position));
                    }
                }
            });
        }
    }

    public interface RecyclerViewRow<T> {
        void showData(T item);
    }

    public interface OnItemClickListener<T> {
        void onItemClick(T position);
    }

    public void updateList(List<T> _mDataset){
        this.mDataset= _mDataset;
        notifyDataSetChanged();
    }

    /**
     * Header ViewHolder
     **/
    public class HeaderViewHolder extends RecyclerView.ViewHolder {
        public RecyclerViewRowHeader<V> mRowHeader;

        public HeaderViewHolder(RecyclerViewRowHeader<V> itemView) {
            super((View) itemView);
            mRowHeader = itemView;
        }

        private void onBind(HeaderViewHolder viewHolder) {
            viewHolder.mRowHeader.showData(mHeaderData);
            viewHolder.mRowHeader.setOnTextChangeListener(onTextChangeListener);

            ((View) viewHolder.mRowHeader).setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    if (onItemClickListenerHeader != null) {
                        onItemClickListenerHeader.onItemClickHeader(mHeaderData);
                    }
                }
            });
        }
    }

    public interface RecyclerViewRowHeader<V> {
        void showData(V item);
        void setOnTextChangeListener(OnTextChangeListener onTextChangeListener);

    }

    public interface OnItemClickListenerHeader<V> {
        void onItemClickHeader(V position);
    }

    /**
     * Footer ViewHolder
     **/
    public class FooterViewHolder extends RecyclerView.ViewHolder {
        public RecyclerViewRowFooter<E> mRowFooter;

        public FooterViewHolder(RecyclerViewRowFooter<E> itemView) {
            super((View) itemView);
            mRowFooter = itemView;
        }

        private void onBind(FooterViewHolder viewHolder) {
            viewHolder.mRowFooter.showData(mFooterData);
            ((View) viewHolder.mRowFooter).setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    if (onItemClickListenerFooter != null) {
                        onItemClickListenerFooter.onItemClickFooter(mFooterData);
                    }
                }
            });
        }
    }

    public interface RecyclerViewRowFooter<E> {
        void showData(E item);
    }

    public interface OnItemClickListenerFooter<E> {
        void onItemClickFooter(E position);
    }

    // text search OnTextChangeListener

    public interface OnTextChangeListener {
        void textSearch(String searchText);
    }
}