<template>
    <form ref="form" @submit.prevent="submitForm" autocomplete="off">
        <div class="form-check">
            <input class="form-check-input" type="checkbox" v-model="form.active">
            <label class="form-check-label">
                Active
            </label>
        </div>
        <div class="mb-3">
            <input type="text" class="form-control" autocomplete="off" v-model.lazy="form.name" @blur="v$.form.name.$touch"  aria-describedby="spnNameErrors">
            <div>
                <span id="spnNameErrors" class="form-text text-danger" v-for="error of v$.form.name.$errors">{{ error.$message }}</span>
            </div>
        </div>      
    </form>
</template>
<script>
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers } from '@vuelidate/validators';  
    
    const { withAsync } = helpers;
    const userListNameFormat = helpers.regex(/^[._()-\/#&$@+\w\s]{3,30}$/)

    export default {
        name: "EditUserList",
        emits: ["valid"],
        props: {
            userlist: Object
        },
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: this.userlist
            }
        },               
        mounted: function () {
        },
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (isValid) {
                    this.$emit('valid', this.form);
                } else {
                    return;
                }
            },
            async userListNameNotExists(value) {  
                if (value === '') {
                    return true; 
                };            

                return await axios.get('/User/UserListNameNotExists', { params: { userListID: this.userlist.id, userListName: value } })
                    .then(res => {
                        return Promise.resolve(res.data);
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }              
        },
        validations() {
            return {
                form: {
                    name: {
                        required: helpers.withMessage('List name is required', required),
                        userListNameFormat: helpers.withMessage('Must be between 3 - 30 characters', userListNameFormat),
                        userListNameNotExists: helpers.withMessage('List name already exists', withAsync(this.userListNameNotExists))
                    },
                }
            }
        }
    };
</script>


