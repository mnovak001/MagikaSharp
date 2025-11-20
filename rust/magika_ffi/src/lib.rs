extern crate magika;
use magika::Session;
use std::ffi::{CStr, CString};
use std::os::raw::c_char;

#[unsafe(no_mangle)]
pub extern "C" fn magika_session_new() -> *mut Session {
    Box::into_raw(Box::new(Session::new().unwrap()))
}

#[unsafe(no_mangle)]
pub extern "C" fn magika_session_free(ptr: *mut Session) {
    if !ptr.is_null() {
        unsafe { drop(Box::from_raw(ptr)); }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn magika_identify_file(
    sess: *mut Session,
    path: *const c_char
) -> *const c_char {
    let s = unsafe { &mut *sess };
    let c_str = unsafe { CStr::from_ptr(path) };
    let path_str = c_str.to_str().unwrap();
    let res = s.identify_file_sync(path_str).unwrap();
    let label = res.info().label;
    CString::new(label).unwrap().into_raw()
}

#[unsafe(no_mangle)]
pub extern "C" fn magika_string_free(ptr: *mut c_char) {
    if !ptr.is_null() {
        unsafe { drop(CString::from_raw(ptr)); }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn magika_identify_bytes(
    sess: *mut Session,
    data: *const u8,
    len: usize
) -> *const c_char {

    if sess.is_null() || data.is_null() {
        return std::ptr::null();
    }

    let sess_ref = unsafe { &mut *sess };

    // Turn raw pointer into Rust slice
    let slice = unsafe { std::slice::from_raw_parts(data, len) };

    // Call magika
    let result = match sess_ref.identify_content_sync(slice) {
        Ok(res) => res,
        Err(_) => return std::ptr::null(),
    };

    let label = result.info().label;

    // Return C string
    CString::new(label).unwrap().into_raw()
}
