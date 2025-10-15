import request from './index'

// 上传响应接口
export interface UploadResponse {
  url: string
  filename: string
  size: number
  mimeType: string
}

// 上传文件
export const uploadFile = (formData: FormData): Promise<UploadResponse> => {
  return request.post('/admin/files/upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

// 上传图片
export const uploadImage = (file: File): Promise<UploadResponse> => {
  const formData = new FormData()
  formData.append('file', file)
  return uploadFile(formData)
}

// 删除文件
export const deleteFile = (filename: string): Promise<void> => {
  return request.delete(`/admin/files/${filename}`)
}

