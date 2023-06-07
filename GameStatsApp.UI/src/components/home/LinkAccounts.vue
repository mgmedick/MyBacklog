<template>
    <div>
        <h2 class="text-center mb-3">Welcome to GameStatsApp</h2>
        <form @submit.prevent="updateUsername" autocomplete="off">
            <div>
                <ul class="list-group">
                    <li class="list-group-item list-group-item-danger" v-for="errorMessage in errorMessages">{{ errorMessage }}</li>
                </ul>
            </div>           
            <div class="mb-2">
                <label for="txtUserName" class="form-label">Username</label>
                <input id="txtUserName" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Username" @blur="v$.form.Username.$touch" aria-describedby="spnUserNameErrors">
                <div>
                    <span id="spnUserNameErrors" class="form-text text-danger" v-for="error of v$.form.Username.$errors">{{ error.$message }}</span>
                </div>
            </div>     
        </form> 
    </div>
</template>
<script>
    import { getFormData } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers, sameAs } from '@vuelidate/validators';
    const { withAsync } = helpers;

    const usernameFormat = helpers.regex(/^[._()-\/#&$@+\w\s]{3,30}$/)

    const asyncUsernameNotExists = async (value) => {  
        if(value === '') { return true; };            

        return await axios.get('/Home/UsernameNotExists', { params: { username: value } })
            .then(res => {
                return Promise.resolve(res.data);
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "LinkAccounts",
        props: {
            islinkvalid: Boolean,
            isoauth: Boolean
        },
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    Username: ''
                },
                errorMessages: [],
                showSignUpModal: false
            }
        },
        computed: {
        },
        created: function () {
        },
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                this.loading = true;

                axios.post('/Home/UpdateUsername', formData)
                    .then((res) => {
                        if (res.data.success) {
                            location.href = '/';
                        } else {
                            that.errorMessages = res.data.errorMessages;
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }          
        },
        validations() {
            return {
                form: {
                    Username: {
                        required: helpers.withMessage('Username is required', required),
                        usernameFormat: helpers.withMessage('Must be between 3 - 30 characters', usernameFormat),
                        usernameNotExists: helpers.withMessage('Username already exists', withAsync(asyncUsernameNotExists))
                    },
                }
            }
        }
    };
</script>


