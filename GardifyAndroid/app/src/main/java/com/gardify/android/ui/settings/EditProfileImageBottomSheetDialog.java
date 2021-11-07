package com.gardify.android.ui.settings;

import android.app.Activity;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.FragmentManager;

import com.android.volley.DefaultRetryPolicy;
import com.android.volley.Request;
import com.android.volley.VolleyLog;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.R;
import com.gardify.android.ui.settings.dialog.UploadTypeDialogFragment;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.VolleyMultipartRequest;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;

import org.apache.http.HttpEntity;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.StringBody;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.UiUtils.navigateToFragment;

public class EditProfileImageBottomSheetDialog extends BottomSheetDialogFragment implements View.OnClickListener {

    private static final String ACTION_ARGUMENT = "UPLOAD_TYPE";

    private Button buttonUploadProfile;
    private Button buttonDeleteProfile;
    private Button buttonClose;
    private ProgressBar progressBar;
    byte[] byteArrayImage;
    String uploadImage = APP_URL.ACCOUNT_API + "uploadProfilImg";

    private Resources resources;
    private int uploadType;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            uploadType = getArguments().getInt(ACTION_ARGUMENT);
        }
    }

    public View onCreateView(@NonNull LayoutInflater layoutInflater, ViewGroup container, Bundle savedInstanceState) {

        View root = layoutInflater.inflate(R.layout.fragment_settings_profile_image, container, false);
        resources = getContext().getResources();
        init(root);

        return root;
    }

    public void init(View root){
        buttonUploadProfile = root.findViewById(R.id.button_settings_profileImage_uploadNewProfil);
        buttonDeleteProfile = root.findViewById(R.id.button_settings_profileImage_deleteProfil);
        buttonClose = root.findViewById(R.id.button_settings_profileImage_close);
        progressBar = root.findViewById(R.id.progress_bar_bottom_nav);

        buttonUploadProfile.setOnClickListener(this);
        buttonDeleteProfile.setOnClickListener(this);
        buttonClose.setOnClickListener(this);

    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.button_settings_profileImage_uploadNewProfil:
                dismiss();
                FragmentManager fm = getActivity().getSupportFragmentManager();
                UploadTypeDialogFragment uploadTypeDialogFragment = new UploadTypeDialogFragment();
                uploadTypeDialogFragment.show(fm, "dialog_fragment_setting_upload_type");
                break;
            case R.id.button_settings_profileImage_deleteProfil:
                onSuccessDelete();
                progressBar.setVisibility(View.VISIBLE);
                break;
            case R.id.button_settings_profileImage_close:
                dismiss();
                break;
        }
    }

    private void onSuccessDelete(){

        HashMap<String, String> params = new HashMap<>();
        params.put("imageTitle", "Image");

        Bitmap bitmap = BitmapFactory.decodeResource(getContext().getResources(),
                R.drawable.icon);

        byteArrayImage = getImageBytes(bitmap);

        HttpEntity httpEntity;
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        ContentType contentType = ContentType.create("image/jpeg");
        String fileName = "screenshot.jpg";
        builder.addBinaryBody("imageFile", byteArrayImage, contentType, fileName);
        for (String key : params.keySet()) {
            builder.addPart(key, new StringBody(params.get(key), ContentType.MULTIPART_FORM_DATA.withCharset("UTF-8")));
        }

        httpEntity = builder.build();
        VolleyMultipartRequest myRequest = new VolleyMultipartRequest(Request.Method.POST, uploadImage, response -> {
            if (getContext() != null) {
                Toast.makeText(getContext(), "Bild wurde geändert", Toast.LENGTH_SHORT).show();
                progressBar.setVisibility(View.GONE);
                dismiss();
                refresh();

            }

        }, error -> {
            if (getContext() != null) {
                Toast.makeText(getContext(), error.toString(), Toast.LENGTH_SHORT).show();
                progressBar.setVisibility(View.GONE);
                dismiss();
                refresh();
            }
        }) {
            @Override
            public String getBodyContentType() {
                return httpEntity.getContentType().getValue();
            }

            @Override
            public Map<String, String> getHeaders() {
                ApplicationUser user = PreferencesUtility.getUser(getContext());
                Map<String, String> headers = new HashMap<>();
                headers.put("Authorization", "Bearer " + user.getToken());
                return headers;
            }

            @Override
            public byte[] getBody() {
                ByteArrayOutputStream bos = new ByteArrayOutputStream();
                try {
                    httpEntity.writeTo(bos);
                } catch (IOException e) {
                    VolleyLog.e("IOException writing to ByteArrayOutputStream");
                }
                return bos.toByteArray();
            }

        };

        myRequest.setRetryPolicy(new DefaultRetryPolicy(10, 2,
                DefaultRetryPolicy.DEFAULT_BACKOFF_MULT));

        RequestQueueSingleton.getInstance(getContext()).addToRequestQueue(myRequest);
    }

    @Override
    public int getTheme() {
        return R.style.BaseBottomSheetDialog;
    }


    private void refresh() {
        navigateToFragment(R.id.nav_controller_settings, (Activity) getContext(), true, null);
        dismiss();
    }
}
