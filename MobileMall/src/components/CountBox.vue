<script setup>
import { ref, watch, defineProps } from 'vue'

// 定义 props 和 emits
const props = defineProps({
  modelValue: {
    type: Number,
    default: 1,
  },
})

const emit = defineEmits(['update:modelValue'])

// 内部状态
const count = ref(props.modelValue)

// 监听外部的 modelValue 变化
watch(
  () => props.modelValue,
  (newValue) => {
    count.value = newValue
  },
)

// 增加和减少的函数
const increment = () => {
  console.log('点击+')
  count.value++
  emit('update:modelValue', count.value)
}

const decrement = () => {
  console.log('点击-')
  if (count.value > 1) {
    count.value--
    emit('update:modelValue', count.value)
  }
}

// 处理输入变化的方法
const handleInput = () => {
  const num = parseInt(count.value, 10)
  if (!isNaN(num) && num >= 1) {
    emit('update:modelValue', num)
  } else {
    count.value = 1 // 如果输入不合法，回退到 1
  }
}
</script>

<template>
  <div class="count-box">
    <button @click="decrement">-</button>
    <input type="number" v-model="count" @input="handleInput" class="inp" />
    <button @click="increment">+</button>
  </div>
  <!-- <div class="count-box">
    <van-field name="stepper" class="inp">
      <template #input>
        <van-stepper v-model="count" @input="handleInput" />
      </template>
    </van-field>
  </div> -->
</template>

<style lang="scss" scoped>
.count-box {
  width: 110px;
  display: flex;
  .add,
  .minus {
    width: 30px;
    height: 30px;
    outline: none;
    border: none;
    background-color: #efefef;
  }
  .inp {
    width: 40px;
    height: 30px;
    outline: none;
    border: none;
    margin: 0 5px;
    background-color: #efefef;
    text-align: center;
  }
}
</style>
