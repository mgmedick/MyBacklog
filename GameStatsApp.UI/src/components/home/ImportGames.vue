<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Import Games</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div class="row g-2 justify-content-center mb-3">
                <div v-for="(userAccount, userAccountIndex) in importgamesvm.userAccounts" class="btn-group me-1 useraccount-btn-group" role="group">
                    <button type="button" class="btn btn-outline-dark d-flex justify-content-center align-items-center" @click="onImportGamesClick($event, userAccount)" :disabled="importingUserAccountIDs[userAccount.id]">
                        <font-awesome-icon :icon="getIconClass(userAccount.accountTypeID)" :class="getIconColorClass(userAccount.accountTypeID)" size="xl"/>
                        <div class="mx-auto">
                            <div class="align-self-start">
                                <span class="mx-auto useraccount-btn-text">{{ (importingUserAccountIDs[userAccount.id] ? 'Importing ' : 'Import ') + ((importingUserAccountIDs[userAccount.id]?.isImportAll == 0 || !userAccount.relativeImportLastRunDateString) ? 'All ' : 'Latest ') + userAccount.accountTypeName + ' games' }}</span>
                            </div>
                            <div v-if="userAccount.relativeImportLastRunDateString" class="align-self-end text-xs">
                                <span>{{ 'Last imported ' + userAccount.relativeImportLastRunDateString }}</span>
                            </div>
                        </div>
                        <font-awesome-icon v-if="importingUserAccountIDs[userAccount.id]" icon="fa-solid fa-spinner" spin size="xl"/>
                    </button>
                    <button type="button" class="btn btn-outline-dark dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 50px;" :disabled="importingUserAccountIDs[userAccount.id]">
                        <span class="visually-hidden">Toggle Dropdown</span>
                    </button>                    
                    <ul class="dropdown-menu">
                        <li><a href="#" @click="onIsImportAllClick" class="dropdown-item isimportall-dropdown-item" :class="{ active : !userAccount.relativeImportLastRunDateString }" data-value="0">All</a></li>
                        <li><a href="#" @click="onIsImportAllClick" class="dropdown-item isimportall-dropdown-item" :class="{ active : userAccount.relativeImportLastRunDateString }" data-value="1">Latest</a></li>
                    </ul>
                </div>
            </div>
            <div class="row g-2 justify-content-center">
                <a href="/" class="btn btn-primary mt-3" tabindex="-1" role="button">Back to my games</a>
            </div>
        </div>
    </div>
</template>
<script>
    import { successToast, errorToast } from '../../js/common.js';
    import axios from 'axios';
    
    export default {
        name: "ImportGames",
        props: {
            importgamesvm: Object
        },
        data() {
            return {
                importingUserAccountIDs: JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {}, 
                isImportAll: 0
            }
        },
        computed: {
        },                                    
        watch: {
        },          
        created: function () {
        },        
        mounted: function () {
            var that = this;
            window.addEventListener('importingUserAccountIDsUpdate', (event) => {
                that.importingUserAccountIDs = JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {};
            });

            if (that.importgamesvm.authSuccess != null) {
                if (that.importgamesvm.authSuccess) {
                    var userAccount = that.importgamesvm.userAccounts.find(i => i.accountTypeID == that.importgamesvm.authAccountTypeID);
                    that.ImportGames(userAccount);
                } else {            
                    that.errorToast("Error authorizing account");       
                }
            }
        },        
        methods: {
            getIconClass: function (accountTypeID) {
                var iconClass = '';

                switch (accountTypeID) {
                    case 1:
                        iconClass = 'fa-brands fa-steam';
                        break;
                    case 2:
                        iconClass = 'fa-brands fa-xbox';
                        break;
                }

                return iconClass;
            },
            getIconColorClass: function (accountTypeID) {
                var iconClass = '';

                switch (accountTypeID) {
                    case 1:
                        iconClass = 'text-color-blue';
                        break;
                    case 2:
                        iconClass = 'text-color-green';
                        break;
                }

                return iconClass;
            },  
            onIsImportAllClick(e) {
                var el = e.target;
                var bntGroupEl = el.closest('.useraccount-btn-group');
                bntGroupEl.querySelectorAll('.isimportall-dropdown-item').forEach(item => {
                    item.classList.remove('active');
                });
                el.classList.add('active');
                
                var btnTextEl = bntGroupEl.querySelector('.useraccount-btn-text');
                btnTextEl.innerHTML = btnTextEl.innerHTML.indexOf("All") > -1 ? btnTextEl.innerHTML.replace("All","Latest") : btnTextEl.innerHTML.replace("Latest","All");
            },          
            onImportGamesClick(e, userAccount) {
                var isImportAll = e.target.closest('.useraccount-btn-group').querySelector('.isimportall-dropdown-item.active').getAttribute('data-value');
                this.ImportGames(userAccount, isImportAll)
            },
            ImportGames(userAccount, isImportAll) {
                var that = this;
                if (!that.importingUserAccountIDs.hasOwnProperty(userAccount.id)) {
                    that.importingUserAccountIDs[userAccount.id] = { accountTypeName: userAccount.accountTypeName, isImportAll: isImportAll };
                    sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(that.importingUserAccountIDs));    
                }

                axios.post('/Home/ImportGames', null,{ params: { userAccountID: userAccount.id, isImportAll: isImportAll == 0 } }).then((res) => {
                    if (res.data.success) {
                        successToast("Successfully imported " + userAccount.accountTypeName + " games");
                    } else {
                        errorToast("Error importing " + userAccount.accountTypeName + " games");                                               
                    }

                    delete that.importingUserAccountIDs[userAccount.id];
                    sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(that.importingUserAccountIDs));    
                })
                .catch(err => { console.error(err); return Promise.reject(err); });
            }                                                                                                                    
        }
    };
</script>


