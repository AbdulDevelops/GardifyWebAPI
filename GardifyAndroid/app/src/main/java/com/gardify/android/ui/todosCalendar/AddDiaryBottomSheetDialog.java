package com.gardify.android.ui.todosCalendar;

import android.app.DatePickerDialog;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.Bundle;
import android.text.InputType;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.NetworkResponse;
import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.account.UserMainGarden;
import com.gardify.android.ui.generic.HeaderItemDecoration;
import com.gardify.android.ui.generic.recyclerItem.GenericGridItem;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestImageUpload;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.viewModelData.todos.TodoCalendarViewModel;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.apache.http.HttpEntity;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.StringBody;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Locale;
import java.util.Map;

import static android.app.Activity.RESULT_OK;
import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.ImageUtils.convertUriToBitmap;
import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;


public class AddDiaryBottomSheetDialog extends BottomSheetDialogFragment implements View.OnClickListener {

    private static final int IMAGE_ALBUM = 1001;
    private static final String TAG = "AddDiaryBottomSheetDialog";

    private Button saveTodoBtn, cancelBtn;
    private EditText titleEdt, descriptionEdt, datePickerEditTextDate, notesEdt;
    private ProgressBar progressBar;
    private TodoCalendarViewModel todos;
    private Bundle bundleArguments;
    private LinearLayout diaryNotesLayout, diaryDateLayout, diaryUploadLayout, diaryRecyclerLinearLayout;
    private TextView headerTxt, textViewImageCount;
    private ImageView ImageUploadBtn, deleteImagesButton;
    // image upload
    private ArrayList<Bitmap> imageArrayList = new ArrayList<>();
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private RecyclerView recyclerView;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_todo_add_diary_entry, container, false);

        init(root);

        // edit todos view
        bundleArguments = getArguments();
        setupEditView();
        setupGroupAdapter();

        return root;
    }

    public void init(View root) {
        /* finding views block */
        headerTxt = root.findViewById(R.id.textView_todo_add_diary_header);
        diaryNotesLayout = root.findViewById(R.id.linearLayout_add_diary_notes);
        diaryDateLayout = root.findViewById(R.id.linearLayout_todo_add_diary_date);
        diaryUploadLayout = root.findViewById(R.id.linearLayout_todo_add_diary_upload_section);
        diaryRecyclerLinearLayout = root.findViewById(R.id.linearLayout_add_diary_recyclerView_section);
        recyclerView = root.findViewById(R.id.recycler_view_diary_grid_view);
        deleteImagesButton = root.findViewById(R.id.image_view_diary_delete_icon);
        notesEdt = root.findViewById(R.id.editText_diary_notes);
        saveTodoBtn = root.findViewById(R.id.button_todo_add_diary_save);
        cancelBtn = root.findViewById(R.id.button_todo_add_diary_cancel);
        titleEdt = root.findViewById(R.id.editText_todo_add_diary_title_name);
        descriptionEdt = root.findViewById(R.id.editText_todo_add_diary_desc);
        datePickerEditTextDate = root.findViewById(R.id.editText_todo_add_diary_date);
        ImageUploadBtn = root.findViewById(R.id.image_view_add_diary_todo_upload_icon);
        textViewImageCount = root.findViewById(R.id.text_view_add_diary_todo_image_count);
        progressBar = root.findViewById(R.id.progressbar_addDiaryEntry);

        datePickerEditTextDate.setInputType(InputType.TYPE_NULL);
        Calendar calendar = Calendar.getInstance();
        updateLabel(datePickerEditTextDate, calendar.getTime());
        datePickerEditTextDate.setOnClickListener(this);
        ImageUploadBtn.setOnClickListener(this);
        deleteImagesButton.setOnClickListener(this);
        saveTodoBtn.setOnClickListener(this);
        cancelBtn.setOnClickListener(this);
    }

    private void setupEditView() {
        if (isEditView()) {
            String todoJsonString = bundleArguments.getString("TODO_DIARY");

            if (todoJsonString != null) {
                todos = ApiUtils.getGsonParser().fromJson(todoJsonString, TodoCalendarViewModel.class);
                // update UI
                headerTxt.setText("Tagebucheintrag bearbeiten");
                titleEdt.setText(todos.getTitle());
                descriptionEdt.setText(todos.getDescription());

                //uneditable boxes
                //diaryNotesLayout.setVisibility(View.VISIBLE);
                //notesEdt.setText(todos.getNotes());
                diaryDateLayout.setVisibility(View.GONE);
                diaryUploadLayout.setVisibility(View.GONE);
                diaryRecyclerLinearLayout.setVisibility(View.GONE);

            }
        }
    }

    private boolean isEditView() {
        return bundleArguments != null;
    }

    private void setupGroupAdapter() {
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.addItemDecoration(new HeaderItemDecoration(0, 0));
        recyclerView.setAdapter(groupAdapter);
    }

    @Override
    public void onClick(View view) {

        switch (view.getId()) {

            case R.id.editText_todo_add_diary_date:
                displayDatePicker(datePickerEditTextDate);
                break;
            case R.id.image_view_add_diary_todo_upload_icon:

                pickImageFromGallery();
                break;
            case R.id.image_view_diary_delete_icon:
                showUploadImageLayout();
                break;

            case R.id.button_todo_add_diary_cancel:
                dismiss();
                break;

            case R.id.button_todo_add_diary_save:

                ApplicationUser user = PreferencesUtility.getUser(getContext());
                UserMainGarden userMainGarden = PreferencesUtility.getUserMainGarden(getContext());

                Map<String, String> params = new HashMap<>();
                JSONObject jsonObject;

                if (isNewDiary()) {

                    if (titleEdt.getText().toString().length() < 1) {
                        titleEdt.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    } else if (descriptionEdt.getText().toString().length() < 1) {
                        descriptionEdt.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    } else if (datePickerEditTextDate.getText().toString().length() < 1) {
                        datePickerEditTextDate.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    }

                    String saveTodoUrl = APP_URL.DIARY_API;

                    params.put("Title", titleEdt.getText().toString());
                    params.put("Description", descriptionEdt.getText().toString());
                    params.put("Notes", notesEdt.getText().toString());
                    params.put("Date", datePickerEditTextDate.getText().toString());
                    params.put("UserId", user.getUserId());
                    params.put("EntryObjectId", userMainGarden.getId().toString());
                    params.put("EntryOf", "3");

                    jsonObject = new JSONObject(params);

                    RequestQueueSingleton.getInstance(getContext()).objectRequest(saveTodoUrl, Request.Method.POST, this::onSuccessSaveTodo, this::onError, jsonObject);
                    progressBar.setVisibility(View.VISIBLE);

                } else {
                    if (titleEdt.getText().toString().length() < 1) {
                        titleEdt.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    } else {
                        // updated params
                        params.put("Title", titleEdt.getText().toString());
                        params.put("Description", descriptionEdt.getText().toString());
                        params.put("Notes", notesEdt.getText().toString());
                        // other params
                        params.put("Date", String.valueOf(todos.getDate()));
                        params.put("Id", String.valueOf(todos.getId()));
                        params.put("UserId", PreferencesUtility.getUser(getContext()).getUserId());
                        params.put("EntryObjectId", userMainGarden.getId().toString());
                        params.put("EntryOf", "3");

                        jsonObject = new JSONObject(params);

                        String editTodoUrl = APP_URL.DIARY_API + todos.getId();
                        RequestQueueSingleton.getInstance(getContext()).objectRequest(editTodoUrl, Request.Method.PUT, this::onSuccessSaveTodo, this::onError, jsonObject);
                        progressBar.setVisibility(View.VISIBLE);
                    }
                }
                break;
        }
    }

    private void showUploadImageLayout() {
        imageArrayList.clear();
        groupAdapter.clear();
        diaryUploadLayout.setVisibility(View.VISIBLE);
        diaryRecyclerLinearLayout.setVisibility(View.GONE);
        textViewImageCount.setText("0 Bild(er) ausgewählt");
    }

    private void onSuccessSaveTodo(JSONObject jsonObject) {
        if (isNewDiary() && hasImage()) {

            String diaryId;
            try {
                diaryId = jsonObject.getString("Id");
                Iterator<Bitmap> iterator = imageArrayList.iterator();
                while (iterator.hasNext()) {
                    Bitmap bitmap = iterator.next();
                    uploadImage(diaryId, bitmap);
                    if (!iterator.hasNext()) {
                        //last item
                        if (isVisible()) {
                            progressBar.setVisibility(View.GONE);
                            Toast.makeText(getContext(), "Eintrag hinzugefügt", Toast.LENGTH_SHORT).show();
                            dismiss();
                            navigateToFragment(R.id.nav_controller_todo, getActivity(), true, null);
                        }
                    }
                }

            } catch (JSONException e) {
                e.printStackTrace();
            }
        } else {
            progressBar.setVisibility(View.GONE);
            Toast.makeText(getContext(), "Eintrag hinzugefügt", Toast.LENGTH_SHORT).show();
            dismiss();
            navigateToFragment(R.id.nav_controller_todo, getActivity(), true, null);

        }

    }

    private void onError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
        progressBar.setVisibility(View.GONE);
    }

    private boolean hasImage() {
        return imageArrayList.size() > 0;
    }

    private boolean isNewDiary() {
        return bundleArguments == null;
    }

    private void uploadImage(String todoId, Bitmap bitmap) {

        Map<String, String> params = new HashMap<>();

        HttpEntity httpEntity;
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        params.put("Id", todoId);
        params.put("ImageTitle", "Image_" + new Date().getTime() + ".jpg");

        // Add binary body
        ContentType contentType = ContentType.create("image/jpeg");
        String fileName = "todo_image_" + new Date().getTime() + ".jpg";
        if (bitmap != null) {
            builder.addBinaryBody("ImageFile", getImageBytes(bitmap), contentType, fileName);
        }
        // adding params
        for (String key : params.keySet()) {
            builder.addPart(key, new StringBody(params.get(key), ContentType.MULTIPART_FORM_DATA.withCharset("UTF-8")));
        }

        httpEntity = builder.build();
        String addTodoUrl = APP_URL.DIARY_API + "upload";


        Request request = new RequestImageUpload(getContext())
                .imageRequest(addTodoUrl, this::onSuccessImageUpload, this::onErrorImageUpload, httpEntity);

        RequestQueueSingleton.getInstance(getContext()).addToRequestQueue(request);

    }

    private void onErrorImageUpload(VolleyError volleyError) {
        Toast.makeText(getContext(), volleyError.toString(), Toast.LENGTH_SHORT).show();
    }

    private void onSuccessImageUpload(NetworkResponse networkResponse) {
        String resultResponse = new String(networkResponse.data);
        Log.d(TAG, "image successfully uploaded");
    }

    private void pickImageFromGallery() {
        Intent intent = new Intent();
        intent.setType("image/*");
        intent.putExtra(Intent.EXTRA_ALLOW_MULTIPLE, true);
        intent.setAction(Intent.ACTION_GET_CONTENT);
        startActivityForResult(Intent.createChooser(intent, "Select Picture"), IMAGE_ALBUM);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);// FOR CHOOSING MULTIPLE IMAGES
        try {
            if (requestCode == IMAGE_ALBUM && resultCode == RESULT_OK && null != data) {
                addImagesToArrayList(data);
            }
        } catch (Exception e) {
            Toast.makeText(getContext(), R.string.toDoCalendar_errorSomethingWentWrong, Toast.LENGTH_SHORT).show();
        }
        textViewImageCount.setText(imageArrayList.size() + " Bild(er) ausgewählt");
        showImageGridView();
    }

    private void addImagesToArrayList(Intent data) {
        if (data.getClipData() != null) {
            int count = data.getClipData().getItemCount(); //evaluate the count before the for loop --- otherwise, the count is evaluated every loop.
            for (int i = 0; i < count; i++) {
                Uri imageUri = data.getClipData().getItemAt(i).getUri();
                Bitmap bitmap = convertUriToBitmap(getContext(), imageUri);
                imageArrayList.add(bitmap);
                Log.e(TAG, "onActivityResult: " + imageUri);
            }
        } else if (data.getData() != null) {
            Uri imageUri = data.getData();
            Bitmap bitmap = convertUriToBitmap(getContext(), imageUri);
            imageArrayList.add(bitmap);
            Log.e(TAG, "onActivityResult: " + imageUri);
        }
    }

    GenericGridItem gridItem;
    private void showImageGridView() {
        diaryRecyclerLinearLayout.setVisibility(View.VISIBLE);
        diaryUploadLayout.setVisibility(View.GONE);
        Section imageSection = new Section();
        for (int i = 0; i < imageArrayList.size(); i++) {

            gridItem = new GenericGridItem.Builder(getContext())
                    .setId(i)
                    .setBitmap(imageArrayList.get(i))
                    .setSpanCount(3)
                    .build();

            imageSection.add(gridItem);
        }
        groupAdapter.add(imageSection);
    }

    private void displayDatePicker(EditText editText) {
        final Calendar myCalendar = Calendar.getInstance();

        DatePickerDialog.OnDateSetListener date = new DatePickerDialog.OnDateSetListener() {
            @Override
            public void onDateSet(DatePicker view, int year, int monthOfYear,
                                  int dayOfMonth) {
                myCalendar.set(Calendar.YEAR, year);
                myCalendar.set(Calendar.MONTH, monthOfYear);
                myCalendar.set(Calendar.DAY_OF_MONTH, dayOfMonth);
                updateLabel(editText, myCalendar.getTime());
            }
        };

        new DatePickerDialog(getContext(), date, myCalendar
                .get(Calendar.YEAR), myCalendar.get(Calendar.MONTH),
                myCalendar.get(Calendar.DAY_OF_MONTH)).show();
    }

    private void updateLabel(EditText editText, Date time) {
        String myFormat = "yyyy-MM-dd"; //In which you need put here
        SimpleDateFormat sdf = new SimpleDateFormat(myFormat, Locale.getDefault());
        editText.setText(sdf.format(time));
    }

    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     */
    @Override
    public void onResume() {
        super.onResume();

        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, getActivity(), true, null);
        }
    }

    @Override
    public int getTheme() {
        return R.style.BaseBottomSheetDialog;
    }
}