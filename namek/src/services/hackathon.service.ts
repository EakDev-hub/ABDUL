import apiClient from './api'
import type { SubmitRequest, SubmitResponse } from '@/types/hackathon'

// Mock data สำหรับทดสอบ
const mockResponse: SubmitResponse = {
  uuid: 'xxxxxyyyyyzzzzz',
  passKeyType: 'develop',
  maxDurationInSecs: 20,
  totalQuestion: 20,
  maximumScore: 20,
  answeredQuestion: 9,
  score: 3.7,
  results: [
    {
      no: 1,
      question: 'ลูกค้าอาชีพทหาร สามารถกู้แคมเปญพิเศษได้หรือไม่',
      expectedAnswer: 'กู้ไม่ได้ ลูกค้าที่จะกู้แคมเปญพิเศษได้ ต้องประกอบอาชีพ xx yy zz เท่านั้น',
      actualAnswer: '{"errorCode":"000001","errorMessage":"ไม่พบข้อมูล","errorField":null}',
      score: 0
    },
    {
      no: 2,
      question: 'ลูกค้าอาชีพครู สามารถกู้แคมเปญพิเศษได้หรือไม่',
      expectedAnswer: 'กู้ไม่ได้ ลูกค้าที่จะกู้แคมเปญพิเศษได้ ต้องประกอบอาชีพ xx yy zz เท่านั้น',
      actualAnswer: 'ได้',
      score: 0.8
    },
    {
      no: 3,
      question: 'ลูกค้าอาชีพแพทย์ สามารถกู้แคมเปญพิเศษได้หรือไม่',
      expectedAnswer: 'กู้ได้ ลูกค้าที่จะกู้แคมเปญพิเศษได้ ต้องประกอบอาชีพแพทย์ วิศวกร หรือทนายความ',
      actualAnswer: 'กู้ได้ เพราะเป็นอาชีพที่อยู่ในเกณฑ์',
      score: 0.9
    },
    {
      no: 4,
      question: 'วงเงินกู้สูงสุดสำหรับแคมเปญพิเศษคือเท่าไหร่',
      expectedAnswer: 'วงเงินกู้สูงสุด 500,000 บาท',
      actualAnswer: '500000',
      score: 0.6
    },
    {
      no: 5,
      question: 'อัตราดอกเบี้ยของแคมเปญพิเศษคือเท่าไหร่',
      expectedAnswer: 'อัตราดอกเบี้ย 3.99% ต่อปี',
      actualAnswer: '{"errorCode":"000002","errorMessage":"ไม่สามารถคำนวณได้"}',
      score: 0
    },
    {
      no: 6,
      question: 'ระยะเวลาผ่อนชำระสูงสุดคือกี่เดือน',
      expectedAnswer: 'ระยะเวลาผ่อนชำระสูงสุด 60 เดือน',
      actualAnswer: '60 เดือน',
      score: 1.0
    },
    {
      no: 7,
      question: 'ลูกค้าอายุ 65 ปี สามารถกู้ได้หรือไม่',
      expectedAnswer: 'กู้ไม่ได้ เนื่องจากเกินอายุที่กำหนด (สูงสุด 60 ปี)',
      actualAnswer: 'ไม่ได้',
      score: 0.5
    },
    {
      no: 8,
      question: 'เอกสารที่ต้องใช้ในการสมัครมีอะไรบ้าง',
      expectedAnswer: 'บัตรประชาชน, สลิปเงินเดือน 3 เดือนล่าสุด, หนังสือรับรองการทำงาน',
      actualAnswer: 'บัตรประชาชน และสลิปเงินเดือน',
      score: 0.4
    },
    {
      no: 9,
      question: 'ค่าธรรมเนียมในการอนุมัติสินเชื่อคือเท่าไหร่',
      expectedAnswer: 'ไม่มีค่าธรรมเนียมในการอนุมัติสินเชื่อ',
      actualAnswer: 'ฟรี ไม่มีค่าใช้จ่าย',
      score: 0.7
    }
  ]
}

export const hackathonService = {
  async submitAnswer(data: SubmitRequest): Promise<SubmitResponse> {
    // Simulate API call delay
    await new Promise(resolve => setTimeout(resolve, 1500))

    // ใช้ mock data สำหรับทดสอบ
    // เมื่อต้องการเรียก API จริง ให้ uncomment บรรทัดด้านล่างและ comment mock response
    // const response = await apiClient.post<SubmitResponse>('/hackathon/submit', data)
    // return response.data

    // Return mock data with team info
    return {
      ...mockResponse,
      uuid: `${data.team}-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
    }
  }
}