package com.gardify.android.generic;

import android.content.Context;
import android.widget.ArrayAdapter;
import android.widget.Filter;
import android.widget.Filterable;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import java.util.LinkedHashMap;
import java.util.Map;

/**
 * Adapter class that is used for the auto-suggest feature in favourite teams and favourite leagues.
 * Stores a HashMap instead of a list because we need the underlying IDs, not the selected text.
 */
public class AutoSuggestAdapter<T> extends ArrayAdapter<String> implements Filterable {
    private LinkedHashMap<String, T> mlistData;

    public AutoSuggestAdapter(@NonNull Context context, int resource) {
        super(context, resource);
        mlistData = new LinkedHashMap<>();
    }

    public void setData(LinkedHashMap<String, T> list) {
        mlistData.clear();
        mlistData.putAll(list);
    }

    @Override
    public int getCount() {
        return mlistData.size();
    }

    @Nullable
    @Override
    public String getItem(int position) {
        return getObject(position).getKey();
    }

    /**
     * Used to Return the full object directly from adapter.
     *
     * @param position Index of the requested object
     * @return The object at the given index
     */
    public Map.Entry<String, T> getObject(int position) {
        Map.Entry<String, T> entry = (Map.Entry<String, T>) mlistData.entrySet().toArray()[position];
        return entry;
    }

    @NonNull
    @Override
    public Filter getFilter() {
        Filter dataFilter = new Filter() {
            @Override
            protected FilterResults performFiltering(CharSequence constraint) {
                FilterResults filterResults = new FilterResults();
                if (constraint != null) {
                    filterResults.values = mlistData;
                    filterResults.count = mlistData.size();
                }
                return filterResults;
            }

            @Override
            protected void publishResults(CharSequence constraint, FilterResults results) {
                if (results != null && (results.count > 0)) {
                    notifyDataSetChanged();
                } else {
                    notifyDataSetInvalidated();
                }
            }
        };
        return dataFilter;
    }
}
