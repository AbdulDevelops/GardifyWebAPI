package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;
import android.text.Html;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewPlantSearchPaginationBinding;
import com.xwray.groupie.databinding.BindableItem;

public class PaginationRow extends BindableItem<RecyclerViewPlantSearchPaginationBinding> {

    private OnPaginationClickListener onPaginationClickListener;
    private Context context;
    private int pageNumber;
    private int totalPageCount;
    private int takeCount;
    private int plantCount;

    public PaginationRow(Context context, int pageNumber, int totalPageCount, int takeCount, int plantCount,
                         OnPaginationClickListener onPaginationClickListener) {
        this.context = context;
        this.pageNumber = pageNumber;
        this.totalPageCount = totalPageCount;
        this.plantCount = plantCount;
        this.takeCount = takeCount;
        this.onPaginationClickListener = onPaginationClickListener;
    }

    public PaginationRow() {
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_plant_search_pagination;
    }

    @Override
    public void bind(@NonNull final RecyclerViewPlantSearchPaginationBinding binding, int position) {

        int _currentPage = pageNumber + 1; // page number starts from 1
        String paginationRange = paginationRange(_currentPage);
        binding.textViewPaginationNumber.setText(Html.fromHtml(paginationRange));

        binding.buttonPaginationNext.setOnClickListener(v -> {
            onPaginationClickListener.onClick(binding, binding.buttonPaginationNext);
        });
        binding.buttonPaginationPrevious.setOnClickListener(v -> {
            onPaginationClickListener.onClick(binding, binding.buttonPaginationPrevious);
        });
        binding.buttonPaginationFirst.setOnClickListener(v -> {
            onPaginationClickListener.onClick(binding, binding.buttonPaginationFirst);
        });
        binding.buttonPaginationLast.setOnClickListener(v -> {
            onPaginationClickListener.onClick(binding, binding.buttonPaginationLast);
        });
    }

    private String paginationRange(int _currentPage) {
        String _paginationRange = "";
        if (plantCount > takeCount) {
            if (_currentPage == 1) {
                _paginationRange = "<b>" + _currentPage + "</b> " + (_currentPage + 1) + "</b> " + (_currentPage + 2);
            } else if (_currentPage == totalPageCount + 1) {
                _paginationRange = "" + (_currentPage - 2) + " " + (_currentPage - 1) + "<b> " + _currentPage + "</b>";
            } else if (_currentPage >= 2) {
                _paginationRange = "" + (_currentPage - 1) + " <b>" + _currentPage + "</b> " + (_currentPage + 1);
            }
        } else {
            _paginationRange= "1";
        }
        return _paginationRange;
    }

    public interface OnPaginationClickListener {
        void onClick(RecyclerViewPlantSearchPaginationBinding binding, View view);
    }
}
