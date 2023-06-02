<template>
    <div>
        <h2 class="text-center">Create Account</h2>
        <form v-if="!islinkvalid" @submit.prevent="submitForm" autocomplete="off">
            <div>
                <ul class="list-group">
                    <li class="list-group-item list-group-item-danger" v-for="errorMessage in errorMessages">{{ errorMessage }}</li>
                </ul>
            </div>
            <div class="mb-2">
                <label for="txtEmail" class="form-label">Email</label>
                <input id="txtEmail" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Email" disabled>
            </div>              
            <div class="mb-2">
                <label for="txtUserName" class="form-label">Username</label>
                <input id="txtUserName" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Username" @blur="v$.form.Username.$touch" aria-describedby="spnUserNameErrors">
                <div>
                    <span id="spnUserNameErrors" class="form-text text-danger" v-for="error of v$.form.Username.$errors">{{ error.$message }}</span>
                </div>
            </div>
            <div class="mb-2">
                <label for="txtPassword" class="form-label">Password</label>
                <input id="txtPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.Password" @blur="v$.form.Password.$touch" aria-describedby="spnPasswordErrors">
                <div>
                    <span id="spnPasswordErrors" class="form-text text-danger" v-for="error of v$.form.Password.$errors">{{ error.$message }}</span>
                </div>
            </div>
            <div class="mb-3">
                <label for="txtConfirmPassword" class="form-label">Confirm Password</label>
                <input id="txtConfirmPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.ConfirmPassword" @blur="v$.form.ConfirmPassword.$touch" aria-describedby="spnConfirmPasswordErrors">
                <div>
                    <span id="spnConfirmPasswordErrors" class="form-text text-danger" v-for="error of v$.form.ConfirmPassword.$errors">{{ error.$message }}</span>
                </div>
            </div>
            <div class="row g-2 justify-content-center mb-3">
                <button type="submit" class="btn btn-primary">Submit</button>
            </div>      
        </form>
        <div v-else class="text-center">
            <div class="m-3">
                <font-awesome-icon icon="fa-solid fa-hourglass-end" size="2xl" />
            </div>
            <div>
                Activation link has expired, please <a href="/Home/Signup">Sign Up</a> to try again.
            </div>
        </div>    
    </div>
</template>
<script>
    import { getFormData } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers, sameAs } from '@vuelidate/validators';
    const { withAsync } = helpers;

    const usernameFormat = helpers.regex(/^[._()-\/#&$@+\w\s]{3,30}$/)
    const passwordFormat = helpers.regex(/^(?=.*[A-Za-z])(?=.*\d)[._()-\/#&$@+\w\s]{8,30}$/)

    const asyncUsernameNotExists = async (value) => {  
        if(value === '') { return true; };            

        return await axios.get('/Home/UsernameNotExists', { params: { username: value } })
            .then(res => {
                return Promise.resolve(res.data);
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "Activate",
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
                    Username: '',
                    Password: '',
                    ConfirmPassword: ''
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

                axios.post('/Home/Activate', formData)
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
                    Password: {
                        required: helpers.withMessage('Password is required', required),
                        passwordFormat: helpers.withMessage('Must be between 8 - 30 characters with at least 1 number and letter', passwordFormat)
                    },
                    ConfirmPassword: {
                        sameAsPassword: helpers.withMessage('Must match Password', sameAs(this.form.Password))
                    }
                }
            }
        }
    };
</script>


