package com.gardify.android.utils;

import android.util.Log;

import org.jetbrains.annotations.NotNull;

import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;

public class CollectionUtils {
    /**
     * Null check for foreach loop
     **/
    public static List safeCheckList(List other) {
        return other == null ? Collections.EMPTY_LIST : other;
    }

    @NotNull
    public static <T> List<T> removeDuplicates(List<T> list) {
        return list.stream()
                .distinct()
                .collect(Collectors.toList());
    }
}
