package com.gardify.android.ui.todosCalendar.recyclerItems;

import android.content.Context;
import android.util.Base64;
import android.util.Log;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerItemTodoCardBinding;
import com.gardify.android.ui.todosCalendar.TodoFragment;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.viewModelData.todos.TodoCalendarViewModel;
import com.xwray.groupie.databinding.BindableItem;

import org.json.JSONObject;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;
import static com.gardify.android.ui.todosCalendar.TodoFragment.TODO_TYPE_ECO_SCAN;
import static com.gardify.android.ui.todosCalendar.TodoFragment.getTodoBasedOnReference;
import static com.gardify.android.utils.StringUtils.formatHtmlKTags;
import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;


public class CardTodoWithImage extends BindableItem<RecyclerItemTodoCardBinding> {

    private int bgColor;
    private Context context;
    private OnCardClickListener onCardClickListener;
    private TodoCalendarViewModel todo;

    public CardTodoWithImage(Context context, int bgColor, TodoCalendarViewModel todo) {
        this.bgColor = bgColor;
        this.todo = todo;
        this.context = context;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_item_todo_card;
    }

    @Override
    public void bind(@NonNull RecyclerItemTodoCardBinding viewBinding, int position) {
        viewBinding.imageView.setVisibility(View.GONE);
        viewBinding.linearLayout.setBackgroundColor(context.getResources().getColor(bgColor, null));
        if (todo.getReferenceType() == TODO_TYPE_ECO_SCAN) {
            String parsedDescription= parseBase64Description(todo.getDescription());
            viewBinding.description.setText(formatHtmlKTags(parsedDescription));
        } else {
            viewBinding.description.setText(formatHtmlKTags(todo.getDescription()));
            if (todo.getImageUrl() != null) {
                viewBinding.imageView.setVisibility(View.VISIBLE);
                String completeImageUrl = returnBaseUrl() + todo.getImageUrl();
                loadImageUsingGlide(context, completeImageUrl, viewBinding.imageView);
            }
        }
        viewBinding.notes.setText(todo.getNotes());

    }

    private String parseBase64Description(String base64Description) {
        byte[] bytes = Base64.decode(base64Description, Base64.DEFAULT);
        JSONObject obj;
        StringBuilder result = new StringBuilder();
        try {
            obj = new JSONObject(new String(bytes));
            String greenedArea = String.valueOf(obj.get("gardenArea"));
            String outdoorArea = String.valueOf(obj.get("totalArea"));
            String usedArea = String.valueOf(obj.get("areaRating"));
            String ecologicalCriteria = String.valueOf(obj.get("gardenRating"));
            String plantDiversity = String.valueOf(obj.get("plantsRating"));

            result.append("Begrünte Fläche: ").append(greenedArea).append("m").append("\u00B2").append("<br />");
            result.append("Außenfläche: ").append(outdoorArea).append("m").append("\u00B2").append("<br />");
            result.append("Flächennutzung: ").append(usedArea).append(" % ").append("<br />");
            result.append("Ökokriterien: ").append(ecologicalCriteria).append(" % ").append("<br />");
            result.append("Pflanzenvielfalt: ").append(plantDiversity).append(" % ").append("<br />");

        } catch (Throwable t) {
            Log.e("CardTodoWithImage:", t.getMessage());
        }
        return result.toString();
    }

    private String returnBaseUrl() {
        TodoFragment.ToDoType toDoType = getTodoBasedOnReference(todo.getReferenceType());
        switch (toDoType) {
            case diary:
            case custom:
                return APP_URL.BASE_ROUTE;
            case todo:
            case scan:
                return APP_URL.BASE_ROUTE_INTERN;
        }
        return APP_URL.BASE_ROUTE_INTERN;
    }

    public interface OnCardClickListener {
        void onClick(int textResId);
    }
}
