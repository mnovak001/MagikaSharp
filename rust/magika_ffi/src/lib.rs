extern crate magika;
use magika::Session;
use std::ffi::{CStr, CString};
use std::os::raw::c_char;

#[repr(C)]
pub struct CTypeInfo {
    pub label: *const c_char,
    pub mime_type: *const c_char,
    pub group: *const c_char,
    pub description: *const c_char,
    pub extensions: *const *const c_char,
    pub extensions_len: usize,
    pub is_text: bool,
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn magika_typeinfo_free(ptr: *mut CTypeInfo) {
    if ptr.is_null() {
        return;
    }

    unsafe {
        let info = Box::from_raw(ptr);

        // Free main strings
        drop(CString::from_raw(info.label as *mut c_char));
        drop(CString::from_raw(info.mime_type as *mut c_char));
        drop(CString::from_raw(info.group as *mut c_char));
        drop(CString::from_raw(info.description as *mut c_char));

        // Free extension strings
        let ext_slice = std::slice::from_raw_parts(info.extensions, info.extensions_len);
        for &ext in ext_slice {
            drop(CString::from_raw(ext as *mut c_char));
        }

        // Free the boxed slice of pointers
        let slice_ptr = std::ptr::slice_from_raw_parts_mut(
            info.extensions as *mut *const c_char,
            info.extensions_len,
        );
        drop(Box::from_raw(slice_ptr));
    };
}

fn convert_typeinfo(info: &magika::TypeInfo) -> *mut CTypeInfo {
    // Convert strings
    let label = CString::new(info.label).unwrap().into_raw();
    let mime_type = CString::new(info.mime_type).unwrap().into_raw();
    let group = CString::new(info.group).unwrap().into_raw();
    let description = CString::new(info.description).unwrap().into_raw();

    // Extensions
    let mut ext_vec: Vec<*const c_char> = Vec::new();
    for ext in info.extensions {
        ext_vec.push(CString::new(*ext).unwrap().into_raw());
    }

    let ext_slice_box = ext_vec.into_boxed_slice();
    let ext_ptr = ext_slice_box.as_ptr();
    let ext_len = ext_slice_box.len();

    Box::leak(ext_slice_box);

    Box::into_raw(Box::new(CTypeInfo {
        label,
        mime_type,
        group,
        description,
        extensions: ext_ptr,
        extensions_len: ext_len,
        is_text: info.is_text,
    }))
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn magika_session_new() -> *mut Session {
    Box::into_raw(Box::new(Session::new().unwrap()))
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn magika_session_free(ptr: *mut Session) {
    if !ptr.is_null() {
        unsafe {
            drop(Box::from_raw(ptr));
        }
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn magika_identify_file(
    sess: *mut Session,
    path: *const c_char,
) -> *mut CTypeInfo {
    if sess.is_null() || path.is_null() {
        return std::ptr::null_mut();
    }

    unsafe {
        let s = &mut *sess;
        let c_str = CStr::from_ptr(path);

        let path_str = match c_str.to_str() {
            Ok(s) => s,
            Err(_) => return std::ptr::null_mut(),
        };

        let res = match s.identify_file_sync(path_str) {
            Ok(r) => r,
            Err(_) => return std::ptr::null_mut(),
        };

        return convert_typeinfo(res.info());
    };
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn magika_string_free(ptr: *mut c_char) {
    if !ptr.is_null() {
        unsafe { drop(CString::from_raw(ptr)); };
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn magika_identify_bytes(
    sess: *mut Session,
    data: *const u8,
    len: usize,
) -> *mut CTypeInfo {
    if sess.is_null() || data.is_null() {
        return std::ptr::null_mut();
    }

    unsafe {
        let sess_ref = &mut *sess;
        let slice = std::slice::from_raw_parts(data, len);
        
        let result = match sess_ref.identify_content_sync(slice) {
            Ok(res) => res,
            Err(_) => return std::ptr::null_mut(),
        };
    
        return convert_typeinfo(result.info())
    };
}
